using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dnxt.Extensions;
using Dnxt.Parsing;
using Dnxt.Reflection;
using JetBrains.Annotations;

namespace Dnxt
{
    public class Transformation<T>
    {
        [NotNull]
        private readonly IReadOnlyCollection<KeyValuePair<string, Expression<Func<T, object>>>> _setters;
        [NotNull]
        private readonly Lazy<Func<T, T>> _transformer;
        [NotNull]
        private readonly IEnumerable<KeyValuePair<PropertyInfo, Expression<Func<T, object>>>> _getters;
        [NotNull]
        private readonly IEnumerable<Constructor> _constructors;

        public Transformation() : this(new List<KeyValuePair<string, Expression<Func<T, object>>>>())
        {
        }

        private Transformation([NotNull] IReadOnlyCollection<KeyValuePair<string, Expression<Func<T, object>>>> setters)
        {
            if (setters == null) throw new ArgumentNullException(nameof(setters));

            _setters = setters;
            _transformer = new Lazy<Func<T, T>>(() => GetTransformer(setters));

            var type = typeof(T);
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            _getters = props.Select(prop =>
                new KeyValuePair<PropertyInfo, Expression<Func<T, object>>>(prop, GetPropGetter(prop, setters)));

            _constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                .Select(info => new Constructor(info, info.GetParameters()));
        }

        [NotNull]
        public Transformation<T> Map<TF, TV>(Expression<Func<T, TF>> propGetter, Expression<Func<T, TV>> mapTo)
            where TV : TF
        {
            var prop = GetPropName(propGetter);
            if (prop == null)
            {
                throw new InvalidOperationException("Expression should be MemberExpression.");
            }

            var setters = new List<KeyValuePair<string, Expression<Func<T, object>>>>(_setters)
            {
                new KeyValuePair<string, Expression<Func<T, object>>>(prop, mapTo.ConvertToObject())
            };

            return new Transformation<T>(setters);
        }

        private static string GetPropName<TF>(Expression<Func<T, TF>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;

            var name = memberExpression?.Member.Name;
            return name;
        }

        private Func<T, T> GetTransformer(IEnumerable<KeyValuePair<string, Expression<Func<T, object>>>> setters)
        {
            var matchedCtor = _constructors
                .Select(ctor => new { ctor, argsGetter = GetArgsGetter(ctor, _getters) })
                .FirstOrDefault(arg => arg.argsGetter.All(argGetter => argGetter != null));

            if (matchedCtor == null)
            {
                var ctors = _constructors.Select(ctor => ctor.Parameters.Select(info => info.Name).ToList()).ToList();
                var args = setters.Select(pair => pair.Key).ToList();
                throw new MatchConstructorNotFound(args, ctors);
            }

            return obj =>
            {
                var arg = matchedCtor.argsGetter.Select(func => func.Compile().Invoke(obj)).ToArray();
                var ctor = matchedCtor.ctor;
                var newInst = ctor.ConstructorInfo.Invoke(arg);
                return (T)newInst;
            };
        }

        private static IEnumerable<Expression<Func<T, object>>> GetArgsGetter(
            [NotNull]Constructor ctor,
            [NotNull]IEnumerable<KeyValuePair<PropertyInfo, Expression<Func<T, object>>>> getters)
        {
            var constructorParams = ctor.Parameters;

            var argsGetters = constructorParams
                .Select(arg => new
                {
                    Name = arg.Name.ToPascalCase(),
                    Getter = getters
                    .Select(kv =>
                    {
                        var prop = kv.Key;
                        var nameEq = string.Equals(prop.Name, arg.Name.ToPascalCase());
                        if (!nameEq)
                        {
                            return null;
                        }

                        var typeEq = prop.PropertyType.IsAssignableFrom(arg.ParameterType);
                        if (!typeEq)
                        {
                            return null;
                        }

                        return kv.Value;
                    })
                    .FirstOrDefault(func => func != null)
                }).ToList();

            return argsGetters.Select(arg => arg.Getter);
        }

        private static Expression<Func<T, object>> GetPropGetter(
            [NotNull]PropertyInfo prop,
            [NotNull]IEnumerable<KeyValuePair<string, Expression<Func<T, object>>>> setters)
        {
            var setter = setters.FirstOrDefault(pair => pair.Key == prop.Name);
            if (setter.Key != null)
            {
                return setter.Value;
            }

            Func<object, object> propGetter = o =>
            {
                var getMethod = prop.GetGetMethod();
                return getMethod.Invoke(o, null);
            };
            return arg => propGetter(arg);
        }

        public T Apply([NotNull] T obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            var transformer = _transformer.Value;
            var updated = transformer.Invoke(obj);
            return updated;
        }

        public IDictionary<string, Expression<Func<T, object>>> GetMapping()
        {
            return _setters.ToDictionary(pair => pair.Key, pair => pair.Value);
        }
    }

    public class TransformationResult<T>
    {
        public TransformationResult(IDictionary<string, Expression<Func<T, object>>> mapping, T result)
        {
            Mapping = mapping;
            Result = result;
        }

        public IDictionary<string, Expression<Func<T, object>>> Mapping { get; }
        public T Result { get; }
    }
}
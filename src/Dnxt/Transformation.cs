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
        private readonly IReadOnlyCollection<KeyValuePair<string, Mapping>> _setters;

        [NotNull]
        private readonly Lazy<Func<T, T>> _transformer;

        public Transformation() : this(new List<KeyValuePair<string, Mapping>>())
        {
        }

        private Transformation([NotNull] IReadOnlyCollection<KeyValuePair<string, Mapping>> setters)
        {
            if (setters == null) throw new ArgumentNullException(nameof(setters));

            _setters = setters;
            _transformer = new Lazy<Func<T, T>>(() => GetTransformer(setters));
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
            
            var expression = Expression.Lambda<Func<T, object>>(Expression.Convert(mapTo.Body, typeof(object)), mapTo.Parameters);

            var setters = new List<KeyValuePair<string, Mapping>>(_setters)
            {
                new KeyValuePair<string, Mapping>(prop, new Mapping(expression))
            };

            return new Transformation<T>(setters);
        }

        private string GetPropName<TF>(Expression<Func<T, TF>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;

            var name = memberExpression?.Member.Name;
            return name;
        }

        public static Func<T, T> GetTransformer(IEnumerable<KeyValuePair<string, Mapping>> setters)
        {
            var type = typeof(T);
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var getters = props.Select(prop => new KeyValuePair<PropertyInfo, Mapping>(prop, GetPropGetter(prop, setters)));

            var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                .Select(info => new Constructor(info, info.GetParameters()));

            var matchedCtor = constructors
                .Select(ctor => new { ctor, argsGetter = GetArgsGetter(ctor, getters) })
                .FirstOrDefault(arg => arg.argsGetter.All(argGetter => argGetter != null));

            if (matchedCtor == null)
            {
                var ctors = constructors.Select(ctor => ctor.Parameters.Select(info => info.Name).ToList()).ToList();
                var args = setters.Select(pair => pair.Key).ToList();
                throw new MatchConstructorNotFound(args, ctors);
            }

            return obj =>
            {
                var arg = matchedCtor.argsGetter.Select(func => func.CompiledFunc(obj)).ToArray();
                var ctor = matchedCtor.ctor;
                var newInst = ctor.ConstructorInfo.Invoke(arg);
                return (T)newInst;
            };
        }

        private static IEnumerable<Mapping> GetArgsGetter(
            [NotNull]Constructor ctor,
            [NotNull]IEnumerable<KeyValuePair<PropertyInfo, Mapping>> getters)
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

        private static Mapping GetPropGetter([NotNull]PropertyInfo prop, [NotNull]IEnumerable<KeyValuePair<string, Mapping>> setters)
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
            return new Mapping(arg => propGetter(arg));
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
            return _setters.ToDictionary(pair => pair.Key, pair => pair.Value.Expression);
        }

        public class Mapping
        {
            public Mapping(Expression<Func<T, object>> expression)
            {
                Expression = expression;
                CompiledFunc = expression.Compile();
            }

            public Func<T, object> CompiledFunc { get; }
            public Expression<Func<T, object>> Expression { get; }
        }
    }

    public class TransformationResult<T>
    {
        public TransformationResult(IDictionary<string, Expression<Func<T, object>>> mapping, T resultModel)
        {
            Mapping = mapping;
            ResultModel = resultModel;
        }

        public IDictionary<string, Expression<Func<T, object>>> Mapping { get; }
        public T ResultModel { get; }
    }
}
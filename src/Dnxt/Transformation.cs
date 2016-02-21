using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dnxt.Extensions;
using JetBrains.Annotations;

namespace Dnxt
{
    public class Transformation<T>
    {
        [NotNull]
        private readonly IReadOnlyCollection<KeyValuePair<string, Func<T, object>>> _setters;

        [NotNull]
        private readonly Lazy<Func<T, T>> _transformer;

        public Transformation() : this(new List<KeyValuePair<string, Func<T, object>>>())
        {
        }

        private Transformation([NotNull] IReadOnlyCollection<KeyValuePair<string, Func<T, object>>> setters)
        {
            if (setters == null) throw new ArgumentNullException(nameof(setters));

            _setters = setters;
            _transformer = new Lazy<Func<T, T>>(() => GetTransformer(setters));
        }

        //[NotNull]
        //public Transformation<T> Set<TF>(Expression<Func<T, TF>> propGetter, TF val)
        //{
        //    var prop = GetPropName(propGetter);
        //    if (prop == null)
        //    {
        //        throw new InvalidOperationException("Expression should be MemberExpression.");
        //    }

        //    var setters = new List<KeyValuePair<string, Func<T, object>>>(_setters)
        //    {
        //        new KeyValuePair<string, Func<T, object>>(prop, o => val)
        //    };

        //    return new Transformation<T>(setters);
        //}

        [NotNull]
        public Transformation<T> Set<TF>(Expression<Func<T, TF>> propGetter, Func<T, TF> val)
        {
            var prop = GetPropName(propGetter);
            if (prop == null)
            {
                throw new InvalidOperationException("Expression should be MemberExpression.");
            }

            var setters = new List<KeyValuePair<string, Func<T, object>>>(_setters)
            {
                new KeyValuePair<string, Func<T, object>>(prop, o => val)
            };

            return new Transformation<T>(setters);
        }

        private string GetPropName<TF>(Expression<Func<T, TF>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;

            var name = memberExpression?.Member.Name;
            return name;
        }

        public static Func<T, T> GetTransformer(IEnumerable<KeyValuePair<string, Func<T, object>>> setters)
        {
            var type = typeof(T);
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var getters = props.Select(prop => new KeyValuePair<PropertyInfo, Func<T, object>>(prop, GetPropGetter(prop, setters)));
            var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            var matchedCtor = constructors
                .Select(ctor => new { ctor, argsGetter = GetArgsGetter(ctor, getters) })
                .FirstOrDefault(arg => arg.argsGetter.All(argGetter => argGetter != null));

            if (matchedCtor == null)
            {
                throw new MatchedConstructorNotFound();
            }

            return obj =>
            {
                var arg = matchedCtor.argsGetter.Select(func => func(obj)).ToArray();
                var ctor = matchedCtor.ctor;
                var newInst = ctor.Invoke(arg);
                return (T)newInst;
            };
        }

        private static IEnumerable<Func<T, object>> GetArgsGetter(
            [NotNull]ConstructorInfo ctor,
            [NotNull]IEnumerable<KeyValuePair<PropertyInfo, Func<T, object>>> getters)
        {
            var constructorParams = ctor.GetParameters();

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

                        var typeEq = arg.ParameterType.IsAssignableFrom(prop.PropertyType);
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

        private static Func<T, object> GetPropGetter([NotNull]PropertyInfo prop, [NotNull]IEnumerable<KeyValuePair<string, Func<T, object>>> setters)
        {
            var setter = setters.FirstOrDefault(pair => pair.Key == prop.Name);
            if (setter.Key != null)
            {
                return setter.Value;
            }

            return o =>
            {
                var getMethod = prop.GetGetMethod();
                return getMethod.Invoke(o, null);
            };
        }

        public T Apply([NotNull] T obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            var transformer = _transformer.Value;
            var updated = transformer.Invoke(obj);
            return updated;
        }
    }

    public class MatchedConstructorNotFound : Exception
    {
    }
}
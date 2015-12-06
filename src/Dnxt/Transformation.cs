using System;
using System.Collections.Concurrent;
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
        private readonly ConcurrentDictionary<string, object> _setters;

        [NotNull]
        private readonly Type _type = typeof(T);

        public Transformation()
        {
            _setters = new ConcurrentDictionary<string, object>();
        }

        [NotNull]
        public Transformation<T> Set<TF>(Expression<Func<T, TF>> propGetter, TF val)
        {
            var prop = GetPropName(propGetter);
            if (prop == null)
            {
                throw new InvalidOperationException("Expression should be MemberExpression.");
            }

            _setters.AddOrUpdate(prop, val, (s, o) => val);
            return this;
        }

        private string GetPropName<TF>(Expression<Func<T, TF>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;

            var name = memberExpression?.Member.Name;
            return name;
        }

        public T Apply([NotNull] T obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            var type = obj.GetType();
            var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            var constructor = constructors.First();
            var constructorParams = constructor.GetParameters();
            var props = _type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var args = constructorParams
                .Select(arg => new
                {
                    Name = arg.Name.ToPascalCase(),
                    Prop = props.FirstOrDefault(prop => string.Equals(prop.Name, arg.Name.ToPascalCase()) && prop.PropertyType == arg.ParameterType)
                }).ToList();

            var notFound = args.Where(pair => pair.Prop == null).ToList();

            if (notFound.Any())
            {
                var first = notFound.First();
                throw new PropertyNotFound(first.Name);
            }

            var values = args
                .Select(x => _setters.ContainsKey(x.Name)
                    ? _setters[x.Name]
                    : x.Prop.GetGetMethod().Invoke(obj, null))
                .ToArray();

            var updated = constructor.Invoke(values);
            return (T)updated;
        }
        
        public class PropertyNotFound : Exception
        {
            public PropertyNotFound(string name) : base(name)
            {
            }
        }
    }
}
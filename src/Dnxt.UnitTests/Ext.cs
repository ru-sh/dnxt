using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Dnxt.DtoGeneration;
using JetBrains.Annotations;

namespace Dnxt.UnitTests
{
    public static class Ext
    {
        public static bool IsNullable([NotNull] this Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static bool IsKey([NotNull] this PropertyModel property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));
            return property.Attributes.OfType<KeyAttribute>().Any();
        }

        public static bool IsNotNull([NotNull] this PropertyModel property)
        {
            var propertyType = property.Type;
            var isValueType = propertyType.IsValueType && !propertyType.IsNullable();
            var required = property.Attributes.OfType<RequiredAttribute>().Any();
            return isValueType || required;
        }
    }
}
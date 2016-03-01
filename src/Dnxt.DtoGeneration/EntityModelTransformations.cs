using System;
using System.Collections.Generic;
using System.Linq;
using Dnxt.DtoGeneration.Transformations;
using JetBrains.Annotations;

namespace Dnxt.DtoGeneration
{
    public static class EntityModelTransformations
    {
        public static Transformation<EntityModel> ForProperties(
            [NotNull] this Transformation<EntityModel> transformation,
            [NotNull] Predicate<PropertyModel> predicate,
            [NotNull] IPropTransformation propTransformation)
        {
            if (transformation == null) throw new ArgumentNullException(nameof(transformation));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            if (propTransformation == null) throw new ArgumentNullException(nameof(propTransformation));

            return transformation.Set(
                model => model.Properties,
                model => model.Properties.Select(Prop).ToList());
        }

        public static PropertyModel Prop(PropertyModel prop)
        {
            return prop;
        }

        //private static List<PropertyModel> Select(
        //    Predicate<PropertyModel> predicate, 
        //    IPropTransformation propTransformation, 
        //    IEnumerable<PropertyModel> props)
        //{
        //    return props.Select(prop =>
        //    {
        //        if (predicate(prop))
        //        {
        //            var propertyModel = propTransformation.Build();
        //            return propertyModel;
        //        }

        //        return prop;
        //    }).ToList();
        //}
    }
}
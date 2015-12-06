using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace Dnxt.DtoGeneration
{
    public class Domain
    {
        [NotNull]
        private readonly Predicate<Type> _isReference;

        [NotNull]
        private readonly Dictionary<Type, EntityModel> _models;

        public Domain([NotNull] Predicate<Type> isReference)
        {
            if (isReference == null) throw new ArgumentNullException(nameof(isReference));
            _isReference = isReference;
            _models = new Dictionary<Type, EntityModel>();
        }

        public EntityModel AddEntityType(Type type)
        {
            lock (_models)
            {
                EntityModel exist;
                if (_models.TryGetValue(type, out exist))
                {
                    return exist;
                }
                
                var propertyInfos = type
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public);

                var props = new Lazy<IReadOnlyList<PropModel>>(() =>
                {
                    return propertyInfos
                    .Where(info => !_isReference(info.PropertyType))
                    .Select(info =>
                    {
                        var attributes = info.GetCustomAttributes(true).Cast<Attribute>().ToArray();
                        var propModel = new PropModel(info.Name, info.PropertyType, attributes);
                        return propModel;
                    }).ToList();
                });

                var refs = new Lazy<IReadOnlyList<RefModel>>(() =>
                {
                    return propertyInfos
                    .Where(info => !_isReference(info.PropertyType))
                    .Select(info =>
                    {
                        var attributes = info.GetCustomAttributes(true).Cast<Attribute>().ToArray();
                        var sourceType = info.PropertyType;
                        var refEntity = AddEntityType(sourceType);
                        var refModel = new RefModel(info.Name, refEntity, attributes);
                        return refModel;
                    }).ToList();
                });
                
                var attrs = new Lazy<IReadOnlyList<object>>(() => type.GetTypeInfo().GetCustomAttributes().ToList());
                var model = new EntityModel(type.Name, props, refs, attrs);
                _models.Add(type, model);
                return model;
            }
        }
    }
}
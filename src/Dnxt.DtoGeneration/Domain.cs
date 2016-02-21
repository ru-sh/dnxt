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
        private readonly Dictionary<string, EntityModel> _entities;

        [NotNull]
        public IReadOnlyDictionary<string, EntityModel> Entities => _entities;

        public Domain([NotNull] Predicate<Type> isReference)
        {
            if (isReference == null) throw new ArgumentNullException(nameof(isReference));
            _isReference = isReference;
            _entities = new Dictionary<string, EntityModel>();
        }

        public Domain AddEntityModel([NotNull] EntityModel entityModel)
        {
            if (entityModel == null) throw new ArgumentNullException(nameof(entityModel));

            _entities.Add(entityModel.Name, entityModel);
            return this;
        }

        public EntityModel AddEntityType<T>()
        {
            var type = typeof(T);
            return AddEntityType(type);
        }

        public EntityModel AddEntityType(Type type)
        {
            lock (_entities)
            {
                var entityName = type.Name;
                EntityModel exist;
                if (_entities.TryGetValue(entityName, out exist))
                {
                    return exist;
                }

                var propertyInfos = type
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public);

                var props = new Lazy<IReadOnlyList<PropertyModel>>(() =>
                {
                    return propertyInfos
                    .Where(info => !_isReference(info.PropertyType))
                    .Select(info =>
                    {
                        var attributes = info.GetCustomAttributes(true).ToArray();
                        var propModel = new PropertyModel(info.Name, info.PropertyType, attributes);
                        return propModel;
                    }).ToList();
                });

                var refs = new Lazy<IReadOnlyList<RefModel>>(() =>
                {
                    return propertyInfos
                    .Where(info => _isReference(info.PropertyType))
                    .Select(info =>
                    {
                        var attributes = info.GetCustomAttributes(true).ToArray();
                        var sourceType = info.PropertyType;
                        var refEntity = AddEntityType(sourceType);
                        var refModel = new RefModel(info.Name, refEntity, attributes);
                        return refModel;
                    }).ToList();
                });

                var attrs = new Lazy<IReadOnlyList<object>>(
                    () => type.GetTypeInfo().GetCustomAttributes().ToList());

                var model = new EntityModel(entityName, props, refs, attrs);
                _entities.Add(entityName, model);

                var refModels = refs.Value; // invoke lazy
                var propModels = props.Value; // invoke lazy
                return model;
            }
        }

        public Domain Transform(
            Predicate<EntityModel> predicate,
            [NotNull] [ItemNotNull]Transformation<EntityModel> transformation)
        {
            if (predicate == null)
            {
                predicate = model => true;
            }

            if (transformation == null)
            {
                throw new ArgumentNullException(nameof(transformation));
            }

            var domain = new Domain(_isReference);
            foreach (var updated in Entities.Where(pair => predicate(pair.Value)).Select(kv => transformation.Apply(kv.Value)))
            {
                domain.AddEntityModel(updated);
            }

            return domain;
        }
    }
}
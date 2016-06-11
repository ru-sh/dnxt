using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace Dnxt.DtoGeneration
{
    public class Domain
    {
        public string Namespace { get; }

        [NotNull]
        private readonly Predicate<Type> _isReference;

        [NotNull]
        private readonly Dictionary<string, EntityModel> _entities;

        [NotNull]
        public IReadOnlyDictionary<string, EntityModel> Entities => _entities;

        public Domain(string @namespace, [NotNull] Predicate<Type> isReference)
        {
            Namespace = @namespace;
            if (isReference == null) throw new ArgumentNullException(nameof(isReference));
            _isReference = isReference;
            _entities = new Dictionary<string, EntityModel>();
        }

        public Domain AddEntity([NotNull] EntityModel entityModel)
        {
            if (entityModel == null) throw new ArgumentNullException(nameof(entityModel));

            _entities.Add(entityModel.Name, entityModel);
            return this;
        }

        public EntityModel AddEntity<T>()
        {
            var type = typeof(T);
            return AddEntity(type);
        }

        public EntityModel AddEntity(Type type)
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

                var attrs = type.GetTypeInfo().GetCustomAttributes().ToList();

                var props = new List<PropertyModel>();
                var refs = new List<RefModel>();

                var model = new EntityModel(entityName, props, refs, attrs);
                _entities.Add(entityName, model);

                foreach (var info in propertyInfos)
                {
                    var attributes = info.GetCustomAttributes(true).ToArray();
                    var sourceType = info.PropertyType;

                    if (_isReference(info.PropertyType))
                    {
                        var refEntity = AddEntity(sourceType);
                        var refModel = new RefModel(info.Name, refEntity, attributes);
                        refs.Add(refModel);
                    }
                    else
                    {
                        var propModel = new PropertyModel(info.Name, info.PropertyType, attributes);
                        props.Add(propModel);
                    }
                }

                return model;
            }
        }

        public Domain Transform(
            Predicate<EntityModel> predicate,
            [NotNull] Transformation<EntityModel> transformation)
        {
            if (predicate == null)
            {
                predicate = model => true;
            }

            if (transformation == null)
            {
                throw new ArgumentNullException(nameof(transformation));
            }

            var domain = new Domain(Namespace, _isReference);
            foreach (var kv in Entities)
            {
                var entityModel = kv.Value;
                if (predicate(entityModel))
                {
                    var updated = transformation.Apply(entityModel);
                    domain.AddEntity(updated);
                }
                else
                {
                    domain.AddEntity(entityModel);
                }
            }

            return domain;
        }
    }
}
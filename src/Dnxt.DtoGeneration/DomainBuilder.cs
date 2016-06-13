using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;

namespace Dnxt.DtoGeneration
{
    public class Domain
    {
        public Domain(string ns, IReadOnlyDictionary<string, EntityModel> entities)
        {
            Namespace = ns;
            Entities = entities;
        }

        public string Namespace { get; }

        [NotNull]
        public IReadOnlyDictionary<string, EntityModel> Entities { get; }
    }

    public class DomainBuilder
    {
        public string Namespace { get; }

        [NotNull]
        private readonly Predicate<Type> _isReference;

        [NotNull]
        private readonly Dictionary<string, EntityModel> _entities;

        [NotNull]
        public IReadOnlyDictionary<string, EntityModel> Entities => _entities;

        public DomainBuilder(string @namespace, [NotNull] Predicate<Type> isReference)
        {
            Namespace = @namespace;
            if (isReference == null) throw new ArgumentNullException(nameof(isReference));
            _isReference = isReference;
            _entities = new Dictionary<string, EntityModel>();
        }

        public DomainBuilder AddEntity([NotNull] EntityModel entityModel)
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
                
                var model = new EntityModel(entityName, props, attrs);
                _entities.Add(entityName, model);

                foreach (var info in propertyInfos)
                {
                    var attributes = info.GetCustomAttributes(true).ToArray();
                    var sourceType = info.PropertyType;

                    if (_isReference(info.PropertyType))
                    {
                        var refEntity = AddEntity(sourceType);
                        var propModel = new PropertyModel(info.Name, refEntity, attributes);
                        props.Add(propModel);
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

        public IEnumerable<KeyValuePair<EntityModel, TransformationResult<EntityModel>>> Transform(
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

            foreach (var kv in Entities)
            {
                var entityModel = kv.Value;
                if (predicate(entityModel))
                {
                    var mapping = transformation.GetMapping();
                    var updated = transformation.Apply(entityModel);
                    var tr = new TransformationResult<EntityModel>(mapping, updated);
                    yield return new KeyValuePair<EntityModel, TransformationResult<EntityModel>>(entityModel, tr);
                }
                else
                {
                    IDictionary<string, Expression<Func<EntityModel, object>>> mapping = new Dictionary<string, Expression<Func<EntityModel, object>>>()
                    {
                        {"Name", model => model.Name },
                        {"Properties", model => model.Properties },
                        {"Attributes", model => model.Attributes },
                        {"Visibility", model => model.Visibility },
                    };

                    var tr = new TransformationResult<EntityModel>(mapping, entityModel);
                    yield return new KeyValuePair<EntityModel, TransformationResult<EntityModel>>(entityModel, tr);
                }
            }
        }

        public Domain GetDomain()
        {
            return new Domain(Namespace, Entities);
        }
    }

}
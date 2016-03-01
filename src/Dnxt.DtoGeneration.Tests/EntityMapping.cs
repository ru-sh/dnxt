using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace Dnxt.DtoGeneration.Tests
{
    public class EntityMapping
    {
        [NotNull]
        private readonly EntityModel _srcModel;

        [NotNull]
        private readonly IReadOnlyCollection<PropertyMapping> _toDstMappings;

        [NotNull]
        private readonly IReadOnlyCollection<PropertyMapping> _toSrcMappings;

        public EntityMapping(EntityModel srcModel) : this(srcModel, new List<PropertyMapping>(), new List<PropertyMapping>())
        {
        }

        private EntityMapping([NotNull] EntityModel srcModel,
            [NotNull] IEnumerable<PropertyMapping> toDstMappings,
            [NotNull] IEnumerable<PropertyMapping> toSrcMappings
            )
        {
            if (srcModel == null) throw new ArgumentNullException(nameof(srcModel));
            if (toDstMappings == null) throw new ArgumentNullException(nameof(toDstMappings));

            this._srcModel = srcModel;
            _toDstMappings = new List<PropertyMapping>(toDstMappings);
            _toSrcMappings = new List<PropertyMapping>(toSrcMappings);
        }

        public EntityMapping Property<T>([NotNull] string dstPropName)
        {
            if (dstPropName == null) throw new ArgumentNullException(nameof(dstPropName));

            var mappings = GetNewMappings(new PropertyMapping(new PropertyModel(dstPropName, typeof(T), new object[0]), null));
            return new EntityMapping(_srcModel, _toDstMappings, _toSrcMappings);
        }

        public EntityMapping Property(string dstPropName, string srcPropName)
        {
            return Property(dstPropName, srcPropName, "_." + srcPropName, null);
        }

        public EntityMapping Property(string dstPropName, string srcPropName, string f1, string f2)
        {
            var srcMapping = GetMapping(srcPropName, f1);
            var srcMappings = GetNewMappings(srcMapping);

            var dstMapping = GetMapping(dstPropName, f2);
            var dstMappings = GetNewMappings(dstMapping);

            return new EntityMapping(_srcModel, srcMappings, dstMappings);
        }

        public EntityMapping Property<T1, T2>(string dstPropName, string srcPropName, Expression<Func<T1, T2>> f1, Expression<Func<T2, T1>> f2)
        {
            var srcMapping = GetMapping(srcPropName, f1.ToString());
            if (typeof(T2) != srcMapping.Prop.Type)
            {
                throw new Exception($"Property '{srcPropName}' is not a '{typeof(T2)}'.");
            }

            var dstProp = new PropertyModel(dstPropName, srcMapping.Prop.Type, srcMapping.Prop.Attributes);
            var dstPropMapping = new PropertyMapping(dstProp, f1.ToString());
            var mappings = GetNewMappings(dstPropMapping);
            return new EntityMapping(_srcModel, mappings, mappings);
        }

        public EntityModel GenerateDestinationModel([NotNull] string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            return new EntityModel(name, _toDstMappings.Select(mapping => mapping.Prop).ToList(), new List<RefModel>(), new List<object>());
        }

        [NotNull]
        private IEnumerable<PropertyMapping> GetNewMappings(PropertyMapping mapping)
        {
            return GetNewMappings(new[] { mapping });
        }

        [NotNull]
        private IEnumerable<PropertyMapping> GetNewMappings(IEnumerable<PropertyMapping> mappings)
        {
            return _toDstMappings.Concat(mappings);
        }

        private static PropertyMapping GetMapping(string srcPropName, string f)
        {
            var parts = srcPropName.Split('c');
            PropertyModel srcProp = null;

            if (parts.Length > 1)
            {
                EntityModel current = _srcModel;
                foreach (var part in parts)
                {
                    var refModel = current.References.First(model => model.Name == part);
                    if (refModel != null)
                    {
                        current = refModel.Entity;
                    }
                    else
                    {
                        srcProp = current.Properties.First(model => model.Name == part);
                    }
                }

                if (srcProp == null)
                {
                    throw new Exception($"'{srcProp}' is not property path.");
                }
            }
            else
            {
                srcProp = _srcModel.Properties.FirstOrDefault(model => model.Name == srcPropName);
                if (srcProp == null)
                {
                    throw new Exception($"Prop with name '{srcPropName}' not found.");
                }
            }

            return new PropertyMapping(srcProp, f);
        }

        private static void AddProperty()
        {
            
        }

        public EntityMapping Reference(string refName, Func<EntityMapping, EntityMapping> func)
        {
            

            throw new NotImplementedException();
        }
    }
}
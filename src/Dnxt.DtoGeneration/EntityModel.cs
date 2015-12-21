using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Dnxt.DtoGeneration
{
    public class EntityModel
    {
        [NotNull]
        private readonly Lazy<IReadOnlyList<PropModel>> _properties;

        [NotNull]
        private readonly Lazy<IReadOnlyList<RefModel>> _references;

        [NotNull]
        private readonly Lazy<IReadOnlyList<object>> _attributes;

        public EntityModel([NotNull] string name,
            [NotNull] Lazy<IReadOnlyList<PropModel>> properties,
            [NotNull] Lazy<IReadOnlyList<RefModel>> references,
            [NotNull] Lazy<IReadOnlyList<object>> attributes)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (properties == null) throw new ArgumentNullException(nameof(properties));
            if (references == null) throw new ArgumentNullException(nameof(references));
            if (attributes == null) throw new ArgumentNullException(nameof(attributes));

            Name = name;
            _properties = properties;
            _references = references;
            _attributes = attributes;
        }

        public EntityModel(
            [NotNull] string name,
            [NotNull] IReadOnlyList<PropModel> properties,
            [NotNull] IReadOnlyList<RefModel> references,
            [NotNull] IReadOnlyList<object> attributes)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (properties == null) throw new ArgumentNullException(nameof(properties));
            if (references == null) throw new ArgumentNullException(nameof(references));
            if (attributes == null) throw new ArgumentNullException(nameof(attributes));

            Name = name;
            _properties = new Lazy<IReadOnlyList<PropModel>>(() => properties);
            _references = new Lazy<IReadOnlyList<RefModel>>(() => references);
            _attributes = new Lazy<IReadOnlyList<object>>(() => attributes);
        }

        public string Name { get; }
        public IReadOnlyList<PropModel> Properties => _properties.Value;
        public IReadOnlyList<RefModel> References => _references.Value;
        public IReadOnlyList<object> Attributes => _attributes.Value;
    }
}
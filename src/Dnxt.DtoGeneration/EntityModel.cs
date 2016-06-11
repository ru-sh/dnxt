using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Dnxt.DtoGeneration
{
    public class EntityModel
    {
        public EntityModel(
            [NotNull] string name,
            [ItemNotNull]IReadOnlyList<PropertyModel> properties = null,
            [ItemNotNull]IReadOnlyList<RefModel> references = null,
            [ItemNotNull]IReadOnlyList<object> attributes = null,
            Visibility visibility = Visibility.Public)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            Name = name;

            Properties = properties ?? new PropertyModel[0];
            References = references ?? new RefModel[0];
            Attributes = attributes ?? new object[0];

            Visibility = visibility;
        }

        public Visibility Visibility { get; }

        [NotNull]
        public string Name { get; }

        [NotNull]
        [ItemNotNull]
        public IReadOnlyCollection<PropertyModel> Properties { get; }

        [NotNull]
        [ItemNotNull]
        public IReadOnlyCollection<RefModel> References { get; }

        [NotNull]
        [ItemNotNull]
        public IReadOnlyCollection<object> Attributes { get; }
    }
}
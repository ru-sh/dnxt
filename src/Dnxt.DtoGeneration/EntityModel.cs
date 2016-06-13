using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Dnxt.DtoGeneration
{
    public class EntityModel
    {
        public EntityModel(
            [NotNull] string name,
            [ItemNotNull]IReadOnlyCollection<PropertyModel> properties = null,
            [ItemNotNull]IReadOnlyCollection<object> attributes = null,
            Visibility visibility = Visibility.Public)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            Name = name;
            Properties = properties ?? new PropertyModel[0];
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
        public IReadOnlyCollection<object> Attributes { get; }
    }

    public class ReferenceAttribute : Attribute
    {
        public EntityModel Target { get; set; }
    }
}
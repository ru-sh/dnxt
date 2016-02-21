using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Dnxt.DtoGeneration
{
    public class PropertyModel
    {
        public PropertyModel([NotNull] string name, [NotNull] Type type, [NotNull] IReadOnlyCollection<object> attributes)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (attributes == null) throw new ArgumentNullException(nameof(attributes));

            Name = name;
            Type = type;
            Attributes = attributes;
        }

        [NotNull]
        public string Name { get; }

        [NotNull]
        public Type Type { get; }

        [NotNull]
        public IReadOnlyCollection<object> Attributes { get; }
    }
}
using System;
using JetBrains.Annotations;

namespace Dnxt.DtoGeneration
{
    public class PropModel
    {
        public PropModel([NotNull] string name, [NotNull] Type type, [NotNull] Attribute[] attributes)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (attributes == null) throw new ArgumentNullException(nameof(attributes));

            Name = name;
            Type = type;
            Attributes = attributes;
        }

        public string Name { get; }
        public Type Type { get; }
        public Attribute[] Attributes { get; }
    }
}
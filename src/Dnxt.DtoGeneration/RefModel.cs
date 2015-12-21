using System;
using JetBrains.Annotations;

namespace Dnxt.DtoGeneration
{
    public class RefModel
    {
        public RefModel([NotNull] string name, [NotNull] EntityModel entity, [NotNull] Attribute[] attributes)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (attributes == null) throw new ArgumentNullException(nameof(attributes));

            Name = name;
            Entity = entity;
            Attributes = attributes;
        }

        public string Name { get; }
        public EntityModel Entity { get; }
        public Attribute[] Attributes { get; }
    }
}
using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Dnxt.DtoGeneration
{
    public class RefModel
    {
        public RefModel([NotNull] string name, [NotNull] EntityModel entity, [NotNull] IReadOnlyCollection<object> attributes)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (attributes == null) throw new ArgumentNullException(nameof(attributes));

            Name = name;
            Entity = entity;
            Attributes = attributes;
        }

        [NotNull]
        public string Name { get; }

        [NotNull]
        public EntityModel Entity { get; }

        [NotNull]
        public IReadOnlyCollection<object> Attributes { get; }
    }
}
using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Dnxt.DtoGeneration
{
    public class RefModel : IPropertyModel
    {
        public RefModel([NotNull] string name, [NotNull] EntityModel entity, IReadOnlyCollection<object> attributes = null, bool hasGetter = true, bool hasSetter = false)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            Name = name;
            Entity = entity;
            Attributes = attributes ?? new object[0];
            HasGetter = hasGetter;
            HasSetter = hasSetter;
        }

        public string Name { get; }

        public string TypeFullName => Entity.Name;

        [NotNull]
        public EntityModel Entity { get; }

        public IReadOnlyCollection<object> Attributes { get; }

        public Visibility Visibility { get; }

        public bool HasGetter { get; }

        public bool HasSetter { get; }
    }
}
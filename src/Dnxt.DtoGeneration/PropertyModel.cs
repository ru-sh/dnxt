using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Dnxt.DtoGeneration
{
    public class PropertyModel<T> : PropertyModel
    {
        public PropertyModel([NotNull] string name, IReadOnlyCollection<object> attributes = null, Visibility visibility = Visibility.Public)
            : base(name, typeof(T), attributes, visibility)
        {
        }
    }

    public class PropertyModel : IPropertyModel
    {
        public PropertyModel([NotNull] string name, [NotNull] EntityModel entityModel, IReadOnlyCollection<object> attributes = null, Visibility visibility = Visibility.Public)
            : this(name, entityModel.Name, attributes.Concat(new[] { new ReferenceAttribute() { Target = entityModel } }).ToList(), visibility)
        { }

        public PropertyModel([NotNull] string name, [NotNull] Type type, IReadOnlyCollection<object> attributes = null, Visibility visibility = Visibility.Public)
            : this(name, type.FullName, attributes, visibility)
        { }

        public PropertyModel(
            [NotNull] string name,
            [NotNull] string typeFullName,
            IReadOnlyCollection<object> attributes = null,
            Visibility visibility = Visibility.Public,
            bool hasGetter = true,
            bool hasSetter = false)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (typeFullName == null) throw new ArgumentNullException(nameof(typeFullName));

            Name = name;
            TypeFullName = typeFullName;
            Attributes = attributes ?? new object[0];
            Visibility = visibility;
            HasGetter = hasGetter;
            HasSetter = hasSetter;
        }

        [NotNull]
        public string Name { get; }

        [NotNull]
        public string TypeFullName { get; }

        [NotNull]
        public IReadOnlyCollection<object> Attributes { get; }

        public Visibility Visibility { get; }
        public bool HasGetter { get; }
        public bool HasSetter { get; }
    }
}
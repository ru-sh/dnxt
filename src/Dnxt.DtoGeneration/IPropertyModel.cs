using System.Collections.Generic;
using JetBrains.Annotations;

namespace Dnxt.DtoGeneration
{
    public interface IPropertyModel
    {
        [NotNull]
        string Name { get; }

        [NotNull]
        string TypeFullName { get; }

        [NotNull]
        IReadOnlyCollection<object> Attributes { get; }

        Visibility Visibility { get; }

        bool HasGetter { get; }

        bool HasSetter { get; }
    }
}
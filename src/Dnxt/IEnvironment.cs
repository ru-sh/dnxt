using Dnxt.Environmenting;
using JetBrains.Annotations;

namespace Dnxt
{
    public interface IEnvironment
    {
        [NotNull]
        IDateTimeProvider DateTime { get; }

        [NotNull]
        string AppDataFolderPath { get; }
    }
}
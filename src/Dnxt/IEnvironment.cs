using Dnxt.Environmenting;
using Dnxt.Logging;
using JetBrains.Annotations;

namespace Dnxt
{
    public interface IEnvironment
    {
        [NotNull]
        IDateTimeProvider DateTime { get; }

        [NotNull]
        ILogger Logger { get; }

        [NotNull]
        string AppDataFolderPath { get; }
    }
}
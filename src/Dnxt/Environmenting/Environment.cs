using System;
using Dnxt.Logging;
using Dnxt.Properties;
using JetBrains.Annotations;

namespace Dnxt.Environmenting
{
    public class Environment : IEnvironment, IDisposable
    {
        [CanBeNull] private readonly string _appDataFolderPath;

        public Environment(string appDataFolderPath = null, IDateTimeProvider dateTime = null, ILogger logger = null)
        {
            _appDataFolderPath = appDataFolderPath;
            DateTime = dateTime ?? new DefaultDateTimeProvider();
            var consoleLogObserver = new ConsoleLogObserver();
            Logger = logger ?? new Logger(consoleLogObserver, DateTime);
        }

        public IDateTimeProvider DateTime { get; }
        public ILogger Logger { get; }

        public string AppDataFolderPath
        {
            get
            {
                if (_appDataFolderPath == null)
                {
                    throw new NullReferenceException(nameof(_appDataFolderPath) + " is not setted.");
                }

                return _appDataFolderPath;
            }
        }

        public void Dispose()
        {
            Logger.Dispose();
        }
    }
}
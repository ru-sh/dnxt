using System;
using System.IO;
using Dnxt.Env;
using Dnxt.Logging;
using Dnxt.Properties;

namespace Dnxt
{
    public class Environment : IEnvironment, IDisposable
    {
        public Environment(string appDataFolderPath = null, IDateTimeProvider dateTime = null, ILogger logger = null)
        {
            DateTime = dateTime ?? new DefaultDateTimeProvider();
            var consoleLogObserver = new ConsoleLogObserver();
            Logger = logger ?? new Logger(consoleLogObserver, DateTime);
            if (appDataFolderPath != null)
            {
                AppDataFolderPath = appDataFolderPath;
            }
            else
            {
                AppDataFolderPath = Directory.GetCurrentDirectory();
            }
        }

        public IDateTimeProvider DateTime { get; }
        public ILogger Logger { get; }
        public string AppDataFolderPath { get; }

        public void Dispose()
        {
            Logger.Dispose();
        }
    }
}
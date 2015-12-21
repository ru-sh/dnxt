using System;
using JetBrains.Annotations;

namespace Dnxt.Environmenting
{
    public class Environment : IEnvironment, IDisposable
    {
        [CanBeNull] private readonly string _appDataFolderPath;

        public Environment(string appDataFolderPath = null, IDateTimeProvider dateTime = null)
        {
            _appDataFolderPath = appDataFolderPath;
            DateTime = dateTime ?? new DefaultDateTimeProvider();
        }

        public IDateTimeProvider DateTime { get; }

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
        }
    }
}
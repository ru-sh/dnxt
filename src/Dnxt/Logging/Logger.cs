using System;
using System.Threading.Tasks;
using Dnxt.Environmenting;
using JetBrains.Annotations;

namespace Dnxt.Logging
{
    public class Logger : ILogger
    {
        private readonly Guid _logId;

        [NotNull]
        private readonly IObserver<LogEntry> _observer;

        [NotNull]
        private readonly IDateTimeProvider _dateTime;

        public Logger([NotNull]IObserver<LogEntry> observer, [NotNull]IDateTimeProvider dateTime) 
            : this(Guid.NewGuid(), observer, dateTime)
        {
        }

        public Logger(Guid logId, [NotNull]IObserver<LogEntry> observer, [NotNull]IDateTimeProvider dateTime)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));
            if (dateTime == null) throw new ArgumentNullException(nameof(dateTime));

            _dateTime = dateTime;
            _logId = logId;
            _observer = observer;
        }

        public async Task<T> LogAsync<T>(Func<Task<T>> asyncFunc, string filePath = null, int line = 0, string memberName = null)
        {
            try
            {
                var task = asyncFunc();
                var result = await task;
                return result;
            }
            catch (Exception e)
            {
                Log(e);
                throw;
            }
        }

        public Task LogAsync(Func<Task> asyncAction, string filePath = null, int line = 0, string memberName = null)
        {
            return LogAsync(async () =>
            {
                var task = asyncAction();
                await task;
                return true;
            });
        }

        public void Log(string msg, object info = null, string filePath = null, int line = 0, string memberName = null)
        {
            Log(LogEntryType.Debug, msg, info, filePath, line, memberName);
        }

        public void Log(Exception e, object info = null, string filePath = null, int line = 0, string memberName = null)
        {
            Log(LogEntryType.Exception, e.ToString(), info, filePath, line, memberName);
        }

        public void Dispose()
        {
            _observer.OnCompleted();
        }

        private void Log(LogEntryType type, string message, object info, string filePath, int line, string memberName, string[] categories = null)
        {
            var entry = new LogEntry(_dateTime.UtcNow, message, type, _logId, info, categories, filePath, line, memberName);
            _observer.OnNext(entry);
        }
    }
}
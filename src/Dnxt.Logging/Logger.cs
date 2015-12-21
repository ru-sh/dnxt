using System;
using System.Collections.Generic;
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
        private readonly IDateTimeProvider _dateTimeProvider;

        public Logger([NotNull]IObserver<LogEntry> observer, [NotNull]IDateTimeProvider dateTime)
            : this(Guid.NewGuid(), observer, dateTime)
        {
        }

        public Logger(Guid logId, [NotNull]IObserver<LogEntry> observer, [NotNull]IDateTimeProvider dateTimeProvider)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));
            if (dateTimeProvider == null) throw new ArgumentNullException(nameof(dateTimeProvider));

            _dateTimeProvider = dateTimeProvider;
            _logId = logId;
            _observer = observer;
        }

        public async Task<T> LogAsync<T>(
            string msg,
            Func<ILogger, Task<T>> asyncFunc, 
            object info = null, 
            IReadOnlyCollection<string> categories = null,
            string filePath = null, int line = 0, string memberName = null)
        {
            if (asyncFunc == null) throw new ArgumentNullException(nameof(asyncFunc));

            try
            {
                var logger = new Logger(Guid.NewGuid(), _observer, _dateTimeProvider);
                Log(LogEntryType.Start, msg, info, filePath, line, memberName, categories);
                var task = asyncFunc(logger);
                var result = await task;
                Log(LogEntryType.Finish, msg, info, filePath, line, memberName, categories);
                return result;
            }
            catch (Exception e)
            {
                Log(e);
                throw;
            }
        }

        public Task LogAsync(
            string msg,
            Func<ILogger, Task> asyncAction, 
            object info = null, IReadOnlyCollection<string> categories = null,
            string filePath = null, int line = 0, string memberName = null)
        {
            if (asyncAction == null) throw new ArgumentNullException(nameof(asyncAction));
            return LogAsync(msg, async logger =>
            {
                var task = asyncAction(logger);
                await task;
                return true;
            });
        }

        public void Log(string msg, object info = null, IEnumerable<string> categories = null, string filePath = null, int line = 0, string memberName = null)
        {
            Log(LogEntryType.Debug, msg, info, filePath, line, memberName, categories);
        }

        public void Log(Exception e, object info = null, IEnumerable<string> categories = null, string filePath = null, int line = 0, string memberName = null)
        {
            Log(LogEntryType.Exception, e.ToString(), info, filePath, line, memberName, categories);
        }

        public Task Flush()
        {
            _observer.OnCompleted();
            return Task.FromResult(true);
        }

        public void Dispose()
        {
            _observer.OnCompleted();
        }

        private void Log(LogEntryType type, string message, object info, string filePath, int line, string memberName, IEnumerable<string> categories)
        {
            var entry = new LogEntry(_dateTimeProvider.UtcNow, message, type, _logId, info, categories, filePath, line, memberName);
            _observer.OnNext(entry);
        }
    }
}
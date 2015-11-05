using System;
using Dnxt.Logging;
using Dnxt.Serialization;
using JetBrains.Annotations;

namespace Dnxt.Properties
{
    public class ConsoleLogObserver : IObserver<LogEntry>
    {
        [NotNull]
        private readonly ISerializer<LogEntry> _serializer;

        public ConsoleLogObserver(ISerializer<LogEntry> serializer = null)
        {
            _serializer = serializer ?? new SimpleLogSerializer();
        }

        public void OnNext(LogEntry value)
        {
            var s = _serializer.Serialize(value);
            Console.Out.WriteLineAsync(s);
        }

        public void OnError(Exception error)
        {
            Console.WriteLine(DateTime.UtcNow.ToString("hh:mm:ss") + ": " + error);
        }

        public void OnCompleted()
        {
        }
    }
}
using System;
using Dnxt.Serialization;
using JetBrains.Annotations;

namespace Dnxt.Logging.Serializers
{
    public class SimpleLogSerializer : ISerializer<LogEntry>
    {
        public string Serialize([NotNull] LogEntry entry)
        {
            if (entry == null) throw new ArgumentNullException(nameof(entry));

            return $"{entry.Utc.ToString("hh:mm:ss.fff")}: {entry.Msg}";
        }
    }
}
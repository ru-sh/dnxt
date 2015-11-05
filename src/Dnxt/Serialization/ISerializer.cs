using System;
using Dnxt.Logging;
using JetBrains.Annotations;

namespace Dnxt.Serialization
{
    public interface ISerializer<in T>
    {
        string Serialize(T obj);
    }

    public interface IDeserializer<out T>
    {
        T Deserialize(string obj);
    }

    public class SimpleLogSerializer : ISerializer<LogEntry>
    {
        public string Serialize([NotNull] LogEntry entry)
        {
            if (entry == null) throw new ArgumentNullException(nameof(entry));

            return $"{entry.Utc.ToString("hh:mm:ss.fff")}: {entry.Msg}";
        }
    }
}
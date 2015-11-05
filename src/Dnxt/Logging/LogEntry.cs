using System;
using System.Collections.Generic;

namespace Dnxt.Logging
{
    public class LogEntry
    {
        public LogEntry(DateTime utc, string msg, LogEntryType type, Guid logId, object info, IReadOnlyCollection<string> categories)
        {
            Utc = utc;
            Msg = msg;
            Type = type;
            LogId = logId;
            Info = info;
            Categories = categories;
        }

        public DateTime Utc { get; }
        public string Msg { get; }
        public LogEntryType Type { get; }
        public Guid LogId { get; }
        public object Info { get; }
        public IReadOnlyCollection<string> Categories { get; }
    }
}
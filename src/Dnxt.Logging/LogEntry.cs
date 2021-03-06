﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Dnxt.Logging
{
    public class LogEntry
    {
        public LogEntry(DateTime utc, string message, LogEntryType type, Guid logId, object info, IEnumerable<string> categories, string filePath, int line, string memberName)
        {
            Utc = utc;
            Msg = message;
            Type = type;
            LogId = logId;
            Info = info;
            Categories = categories;
            Line = line;
            FileName = filePath == null ? "" : Path.GetFileName(filePath);
            MemberName = memberName;
        }

        public DateTime Utc { get; }
        public string Msg { get; }
        public LogEntryType Type { get; }
        public Guid LogId { get; }
        public object Info { get; }
        public IEnumerable<string> Categories { get; }
        public string FileName { get; }
        public int Line { get; }
        public string MemberName { get;}
    }
}
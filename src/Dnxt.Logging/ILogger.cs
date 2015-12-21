using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Dnxt.Logging
{
    public interface ILogger : IDisposable
    {
        Task<T> LogAsync<T>(
            [NotNull]string msg,
            [NotNull][ItemNotNull] Func<ILogger, Task<T>> asyncFunc,
            object info = null,
            IReadOnlyCollection<string> categories = null,
            [CallerFilePath] string filePath = null, 
            [CallerLineNumber]int line = 0, 
            [CallerMemberName] string memberName = null);

        Task LogAsync(
            [NotNull]string msg,
            [NotNull][ItemNotNull] Func<ILogger, Task> asyncFunc,
            object info = null,
            IReadOnlyCollection<string> categories = null,
            [CallerFilePath] string filePath = null, 
            [CallerLineNumber]int line = 0, 
            [CallerMemberName] string memberName = null);

        void Log(
            [NotNull] string msg, 
            object info = null, 
            IEnumerable<string> categories = null,
            [CallerFilePath] string filePath = null, 
            [CallerLineNumber]int line = 0, 
            [CallerMemberName] string memberName = null);

        void Log(
            [NotNull] Exception e, 
            object info = null, 
            IEnumerable<string> categories = null,
            [CallerFilePath] string filePath = null, 
            [CallerLineNumber]int line = 0, 
            [CallerMemberName] string memberName = null);

        Task Flush();
    }
}
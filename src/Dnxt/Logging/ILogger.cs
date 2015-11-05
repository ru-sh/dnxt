using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Dnxt.Logging
{
    public interface ILogger : IDisposable
    {
        Task<T> LogAsync<T>(
            [NotNull][ItemNotNull] Func<Task<T>> asyncFunc, 
            [CallerFilePath] string filePath = null, 
            [CallerLineNumber]int line = 0, 
            [CallerMemberName] string memberName = null);

        Task LogAsync(
            [NotNull][ItemNotNull] Func<Task> asyncFunc, 
            [CallerFilePath] string filePath = null, 
            [CallerLineNumber]int line = 0, 
            [CallerMemberName] string memberName = null);

        void Log(
            [NotNull] string msg, 
            object info = null, 
            [CallerFilePath] string filePath = null, 
            [CallerLineNumber]int line = 0, 
            [CallerMemberName] string memberName = null);

        void Log(
            [NotNull] Exception e, 
            object info = null, 
            [CallerFilePath] string filePath = null, 
            [CallerLineNumber]int line = 0, 
            [CallerMemberName] string memberName = null);
    }
}
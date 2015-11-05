using System;
using System.Threading.Tasks;
using Dnxt.Logging;
using JetBrains.Annotations;

namespace Dnxt
{
    public static class TaskExtensions
    {
        public async static Task SuppressTaskCanceledException([NotNull] this Task task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));

            try
            {
                await task.ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
            }
        }

        public static async void NotWaitAndLogExceptions([NotNull] this Task task, [NotNull] ILogger logger)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            try
            {
                await task.ConfigureAwait(false);
            }
            catch (Exception e)
            {
                logger.Log(e);
            }
        }
    }
}
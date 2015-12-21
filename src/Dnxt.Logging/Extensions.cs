using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Dnxt.Logging
{
    public static class Extensions
    {
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
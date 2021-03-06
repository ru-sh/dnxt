﻿using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Dnxt.Extensions
{
    public static class TaskExtensions
    {
        public static async Task SuppressTaskCanceledException([NotNull] this Task task)
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
    }
}
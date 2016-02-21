using System;
using JetBrains.Annotations;

namespace Dnxt.Extensions
{
    public static class AsyncActExt
    {
        public static AsyncFunc<TOut> SetArg<TIn, TOut>([NotNull] this AsyncFunc<TIn, TOut> func, TIn arg)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));

            return token => func(arg, token);
        }

        public static AsyncAction SetArg<TIn>([NotNull] this AsyncAction<TIn> action, TIn arg)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            return token => action(arg, token);
        }
    }
}
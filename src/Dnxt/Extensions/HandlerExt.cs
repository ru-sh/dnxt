using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Dnxt
{
    public static class Ext
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Dnxt
{
    public class Router<TArg> : IRouter<TArg>
    {
        [NotNull]
        private readonly AsyncFunc<TArg, AsyncAction> _func;

        public Router([NotNull] IReadOnlyCollection<IRouter<TArg>> routers)
        {
            if (routers == null) throw new ArgumentNullException(nameof(routers));

            _func = async (arg, token) =>
            {
                Console.WriteLine($"Arg: {arg}");
                var tasks = routers.Select(router => router.FindHandler(arg, token));
                var actions = await Task.WhenAll(tasks);
                var matched = actions.FirstOrDefault(action => action != null);
                //var s = string.Join(", ", actions.Select((action, i) => $"{i} {action != null}"));
                //Console.WriteLine($"Matched: {s}");
                //Console.WriteLine($"Matched: {matched != null}");
                return matched;
            };
        }

        public Router([NotNull] Predicate<TArg> predicate, [NotNull] AsyncAction<TArg> action)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            if (action == null) throw new ArgumentNullException(nameof(action));

            _func = (arg, t) =>
            {
                var result = predicate(arg) ? (token => action(arg, token)) : (AsyncAction)null;
                return Task.FromResult(result);
            };
        }

        public Router([NotNull] Predicate<TArg> predicate, [NotNull] AsyncAction action)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            if (action == null) throw new ArgumentNullException(nameof(action));

            _func = (arg, token) =>
            {
                var result = predicate(arg) ? action : null;
                return Task.FromResult(result);
            };
        }

        [NotNull]
        public AsyncAction<TArg> ToActionWithDefault(AsyncAction<TArg> def)
        {
            AsyncAction<TArg> handler = async (arg, token) =>
            {
                var action = await FindHandler(arg, token) ?? (tkn => def(arg, tkn));
                await action(token);
            };

            return handler;
        }

        public Task<AsyncAction> FindHandler(TArg arg, CancellationToken cancellation)
        {
            return _func(arg, cancellation);
        }
    }
}
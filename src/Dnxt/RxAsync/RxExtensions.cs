using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Dnxt.RxAsync
{
    public static class RxExtensions
    {
        public static IDisposable Subscribe<T>([NotNull] this IAsyncObservable<T> observable,
            [NotNull] AsyncAction<T> action)
        {
            if (observable == null) throw new ArgumentNullException(nameof(observable));
            if (action == null) throw new ArgumentNullException(nameof(action));

            var lambdaObserver = new LambdaObserver<T>(action);
            return observable.Subscribe(lambdaObserver);
        }
    }

    public class LambdaObserver<T> : IAsyncObserver<T>
    {
        [NotNull] private readonly AsyncAction<T> _action;

        public LambdaObserver([NotNull] AsyncAction<T> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            _action = action;
        }

        public Task OnNext(T msg, CancellationToken token)
        {
            return _action(msg, token);
        }

        public Task OnError(Exception e, CancellationToken token)
        {
            return Task.FromResult(true);
        }

        public Task OnCompleted(CancellationToken token)
        {
            return Task.FromResult(true);
        }
    }
}
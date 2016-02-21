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

        public static IAsyncObservable<T> Where<T>(this IAsyncObservable<T> source, Predicate<T> predicate)
        {
            var subj = new Subjects.SequentSubject<T>();
            var obs = new LambdaObserver<T>((obj, token) =>
            {
                if (!token.IsCancellationRequested && predicate(obj))
                {
                    return subj.OnNextAsync(obj, token);
                }

                return Task.FromResult(true);
            });

            source.Subscribe(obs);

            return subj;
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

        public Task OnNextAsync(T msg, CancellationToken token)
        {
            return _action(msg, token);
        }

        public Task OnErrorAsync(Exception e, CancellationToken token)
        {
            return Task.FromResult(true);
        }

        public Task OnCompletedAsync(CancellationToken token)
        {
            return Task.FromResult(true);
        }
    }
}
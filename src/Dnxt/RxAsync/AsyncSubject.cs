using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Dnxt.RxAsync
{
    public class AsyncSubject<T> : IAsyncSubject<T>, IDisposable
    {
        [NotNull]
        [ItemNotNull]
        readonly LinkedList<IAsyncObserver<T>> _observers = new LinkedList<IAsyncObserver<T>>();

        public Task OnNext(T msg, CancellationToken token)
        {
            return ForAll((observer, cancellationToken) => observer.OnNext(msg, cancellationToken), token);
        }

        public Task OnError(Exception e, CancellationToken token)
        {
            return ForAll((observer, cancellationToken) => observer.OnError(e, cancellationToken), token);
        }

        public Task OnCompleted(CancellationToken token)
        {
            return ForAll((observer, cancellationToken) => observer.OnCompleted(token), token);
        }

        private async Task ForAll(AsyncAction<IAsyncObserver<T>> action, CancellationToken token)
        {
            foreach (var observer in _observers)
            {
                await action(observer, token);
            }
        }

        public IDisposable Subscribe(IAsyncObserver<T> observer)
        {
            var node = _observers.AddLast(observer);
            return new Disposable(() => _observers.Remove(node));
        }

        public void Dispose()
        {
            OnCompleted(CancellationToken.None).Wait();
            _observers.Clear();
        }
    }
}
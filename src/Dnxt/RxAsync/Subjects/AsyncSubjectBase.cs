using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Dnxt.RxAsync.Subjects
{
    public abstract class AsyncSubjectBase<T> : IAsyncSubject<T>, IDisposable
    {
        [NotNull]
        [ItemNotNull]
        readonly LinkedList<IAsyncObserver<T>> _observers = new LinkedList<IAsyncObserver<T>>();

        public Task OnNextAsync(T msg, CancellationToken token)
        {
            return ForAll(_observers, (observer, cancellationToken) => observer.OnNextAsync(msg, cancellationToken), token);
        }

        public Task OnErrorAsync(Exception e, CancellationToken token)
        {
            return ForAll(_observers, (observer, cancellationToken) => observer.OnErrorAsync(e, cancellationToken), token);
        }

        public Task OnCompletedAsync(CancellationToken token)
        {
            return ForAll(_observers, (observer, cancellationToken) => observer.OnCompletedAsync(token), token);
        }

        protected abstract Task ForAll(
            [NotNull][ItemNotNull]IEnumerable<IAsyncObserver<T>> observers, 
            [NotNull]AsyncAction<IAsyncObserver<T>> action, 
            CancellationToken token);

        public IDisposable Subscribe(IAsyncObserver<T> observer)
        {
            LinkedListNode<IAsyncObserver<T>> node;
            lock (_observers)
            {
                node = _observers.AddLast(observer);
            }

            return new Disposable(() =>
            {
                lock (_observers)
                {
                    _observers.Remove(node);
                }
            });
        }

        public void Dispose()
        {
            OnCompletedAsync(CancellationToken.None).Wait();

            lock (_observers)
            {
                _observers.Clear();
            }
        }
    }
}
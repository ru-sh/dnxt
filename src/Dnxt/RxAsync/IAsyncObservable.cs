using System;

namespace Dnxt.RxAsync
{
    public interface IAsyncObservable<out TIn>
    {
        IDisposable Subscribe(IAsyncObserver<TIn> observer);
    }
}
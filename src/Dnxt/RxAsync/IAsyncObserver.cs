using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Dnxt.RxAsync
{
    public interface IAsyncObserver<in TIn>
    {
        [NotNull]
        Task OnNext(TIn msg, CancellationToken token);

        [NotNull]
        Task OnError(Exception e, CancellationToken token);

        [NotNull]
        Task OnCompleted(CancellationToken token);
    }
}
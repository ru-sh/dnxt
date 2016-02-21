using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Dnxt.RxAsync
{
    public interface IAsyncObserver<in TIn>
    {
        [NotNull]
        Task OnNextAsync(TIn msg, CancellationToken token);

        [NotNull]
        Task OnErrorAsync(Exception e, CancellationToken token);

        [NotNull]
        Task OnCompletedAsync(CancellationToken token);
    }
}
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Dnxt
{
    public interface IRouter<in TIn, TOut>
    {
        [NotNull]
        [ItemCanBeNull]
        Task<AsyncFunc<TOut>> FindHandlerAsync(TIn args, CancellationToken cancellation);
    }

    public interface IRouter<in TIn>
    {
        [NotNull]
        [ItemCanBeNull]
        Task<AsyncAction> FindHandler(TIn msg, CancellationToken cancellation);
    }
}
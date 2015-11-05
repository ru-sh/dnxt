using System.Threading.Tasks;
using Dnxt.RxAsync;
using JetBrains.Annotations;

namespace Dnxt
{
    public interface IRouter<in TIn, TOut> : IAsyncFunc<TOut>
    {
        [NotNull]
        [ItemCanBeNull]
        Task<AsyncAction<TOut>> GetHandler(TIn msg);
    }

    public interface IRouter<in TIn>
    {
        [NotNull]
        [ItemCanBeNull]
        Task<AsyncAction> TryGetMatchingHandler(TIn msg);
    }
}
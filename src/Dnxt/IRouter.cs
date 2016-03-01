using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Dnxt
{
    public interface IRouter<in TIn, TOut>
    {
        /// <summary>
        /// Task result (AsyncAction) is null when router is not match
        /// </summary>
        /// <param name="args"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        [NotNull]
        [ItemCanBeNull]
        Task<AsyncFunc<TOut>> FindHandlerAsync(TIn args, CancellationToken cancellation);
    }

    public interface IRouter<in TIn>
    {
        /// <summary>
        /// Task result (AsyncAction) is null when router is not match
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        [NotNull]
        [ItemCanBeNull]
        Task<AsyncAction> FindHandler(TIn arg, CancellationToken cancellation);
    }
}
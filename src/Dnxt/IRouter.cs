using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Dnxt
{
    public interface IRouter<in TIn, TOut>
    {
        [NotNull]
        [ItemCanBeNull]
        Task<AsyncAction<TOut>> FindHandler(TIn msg);
    }

    public interface IRouter<in TIn>
    {
        [NotNull]
        [ItemCanBeNull]
        Task<AsyncAction> FindHandler(TIn msg);
    }
}
using System.Threading;
using System.Threading.Tasks;

namespace Dnxt.RxAsync
{
    public interface IAsyncFunc<TOut>
    {
        Task<TOut> Process(CancellationToken token);
    }
}
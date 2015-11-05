using System.Threading;
using System.Threading.Tasks;

namespace Dnxt.RxAsync
{
    public interface IAsyncAction
    {
        Task Process(CancellationToken token);
    }
}
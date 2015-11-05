using System.Threading;
using System.Threading.Tasks;

namespace Dnxt
{
    public delegate Task<TResult> AsyncFunc<TResult>(CancellationToken token);
    public delegate Task<TResult> AsyncFunc<in TArg, TResult>(TArg arg, CancellationToken token);
}
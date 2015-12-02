using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Dnxt
{
    [NotNull]
    public delegate Task<TResult> AsyncFunc<TResult>(CancellationToken token);

    [NotNull]
    public delegate Task<TResult> AsyncFunc<in TArg, TResult>(TArg arg, CancellationToken token);
}
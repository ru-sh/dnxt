using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Dnxt
{
    [NotNull]
    public delegate Task AsyncAction(CancellationToken token);

    [NotNull]
    public delegate Task AsyncAction<in TArg>(TArg arg, CancellationToken token);

    [NotNull]
    public delegate Task AsyncAction<in TArg1, in TArg2>(TArg1 arg1, TArg2 arg2, CancellationToken token);
}
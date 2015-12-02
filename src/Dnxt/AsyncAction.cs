using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Dnxt
{
    [NotNull]
    public delegate Task AsyncAction(CancellationToken token);

    [NotNull]
    public delegate Task AsyncAction<in TArg>(TArg arg, CancellationToken token);
}
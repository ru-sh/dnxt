using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace Dnxt
{
    public delegate Task AsyncAction(CancellationToken token);
    public delegate Task AsyncAction<in TArg>(TArg arg, CancellationToken token);
}
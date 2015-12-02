using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dnxt.RxAsync.Subjects
{
    public class ConcurrentSubject<T> : AsyncSubjectBase<T>
    {
        protected override Task ForAll(IEnumerable<IAsyncObserver<T>> observers, AsyncAction<IAsyncObserver<T>> action, CancellationToken token)
        {
            var tasks = observers.Select(observer => action(observer, token));
            return Task.WhenAll(tasks);
        }
    }
}
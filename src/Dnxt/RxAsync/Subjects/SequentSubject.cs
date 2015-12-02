using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dnxt.RxAsync.Subjects
{
    public class SequentSubject<T> : AsyncSubjectBase<T>
    {
        protected override async Task ForAll(IEnumerable<IAsyncObserver<T>> observers, AsyncAction<IAsyncObserver<T>> action, CancellationToken token)
        {
            foreach (var observer in observers)
            {
                await action(observer, token);
            }
        }
    }
}
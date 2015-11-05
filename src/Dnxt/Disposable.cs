using System;
using JetBrains.Annotations;

namespace Dnxt
{
    public class Disposable : IDisposable
    {
        [NotNull]
        private readonly Action _action;

        public Disposable([NotNull] Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            _action = action;
        }

        public void Dispose()
        {
            _action();
        }
    }
}
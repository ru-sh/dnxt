using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dnxt.RxAsync;
using JetBrains.Annotations;

namespace Dnxt.Parsing
{
    public class RegexParser<T> : IAsyncFunc<string, IReadOnlyCollection<T>>
    {
        [NotNull]
        private readonly Initializer<T> _initializer;

        [NotNull]
        private readonly RegexRouter _router;

        public RegexParser([NotNull]string regexPattern)
        {
            if (regexPattern == null) throw new ArgumentNullException(nameof(regexPattern));
            _router = new RegexRouter(regexPattern);
            _initializer = new Initializer<T>();
        }

        public async Task<IReadOnlyCollection<T>> InvokeAsync(string arg, CancellationToken cancellation)
        {
            var asyncFunc = await _router.FindHandlerAsync(arg, cancellation);
            if (asyncFunc != null)
            {
                var result = new List<T>();
                var matches = await asyncFunc(cancellation);
                foreach (var match in matches)
                {
                    var item = await _initializer.InvokeAsync(match, cancellation);
                    result.Add(item);
                }

                return result;
            }

            throw new MatchNotFoundException(arg, _router.Pattern);
        }
    }
}
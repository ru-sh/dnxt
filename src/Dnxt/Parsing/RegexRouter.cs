using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Dnxt.Parsing
{
    public class RegexRouter : IRouter<string, IReadOnlyCollection<IDictionary<string, string>>>
    {
        [NotNull]
        public string Pattern { get; }

        [NotNull]
        private readonly Regex _regex;

        public RegexRouter([NotNull] string pattern) : this(pattern, RegexOptions.Compiled)
        {
        }

        public RegexRouter([NotNull] string pattern, RegexOptions options)
        {
            Pattern = pattern;
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));
            _regex = new Regex(pattern, options);
        }

        public Task<AsyncFunc<IReadOnlyCollection<IDictionary<string, string>>>> FindHandlerAsync(string args, CancellationToken cancellation)
        {
            return Task.Factory.StartNew(() =>
            {
                var matches = _regex.Matches(args);
                if (matches.Count == 0)
                {
                    return null;
                }

                AsyncFunc<IReadOnlyCollection<IDictionary<string, string>>> findHandlerAsync = (token) =>
                {
                    return Task.Factory.StartNew<IReadOnlyCollection<IDictionary<string, string>>>(() =>
                    {
                        var groupNames = _regex.GetGroupNames();

                        var result = new List<IDictionary<string, string>>();

                        foreach (Match match in matches)
                        {
                            var dictionary = new Dictionary<string, string>();
                            var groups = match.Groups;
                            for (int i = 0; i < groups.Count; i++)
                            {
                                var groupName = groupNames[i];
                                if (groupName == "0") groupName = "_";
                                dictionary.Add(groupName, groups[i].Value);
                            }

                            result.Add(dictionary);
                        }

                        return result;
                    }, token);
                };

                return findHandlerAsync;
            }, cancellation);
        }
    }
}
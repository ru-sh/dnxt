using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Dnxt.Parsing
{
    public class RegexParser<T>
    {
        [NotNull]
        private readonly Regex _regex;

        [NotNull]
        private readonly Initializer<T> _initializer;

        public RegexParser([NotNull]string regexPattern)
        {
            if (regexPattern == null) throw new ArgumentNullException(nameof(regexPattern));
            _regex = new Regex(regexPattern, RegexOptions.Compiled);
            _initializer = new Initializer<T>();
        }

        public IEnumerable<BankEvent> Parse(string str)
        {
            var matches = GetMatches(str);
            foreach (var match in matches)
            {
                
            }

            yield break;
        }

        private IEnumerable<IDictionary<string, string>> GetMatches(string str)
        {
            var groupNames = _regex.GetGroupNames();
            var matches = _regex.Matches(str);
            foreach (Match match in matches)
            {
                var dictionary = new Dictionary<string, string>();
                var groups = match.Groups;
                for (int i = 0; i < groups.Count; i++)
                {
                    var groupName = groupNames[i];
                    dictionary.Add(groupName, groups[i].Value);
                }

                yield return dictionary;
            }
        }
    }
}
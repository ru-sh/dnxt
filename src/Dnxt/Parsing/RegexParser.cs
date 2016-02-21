using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Dnxt.Parsing
{
    public class RegexParser<T>
    {
        [NotNull]
        private readonly Regex _regex;

        private readonly Constructor _constructor;

        public RegexParser([NotNull]string regexPattern)
        {
            if (regexPattern == null) throw new ArgumentNullException(nameof(regexPattern));
            _regex = new Regex(regexPattern, RegexOptions.Compiled);
            var initializer = new Initializer<T>();
            var groupNames = _regex.GetGroupNames().Where(s => s != "0").ToList();
            var matchedCtor = initializer.GetConstructor(groupNames);

            if (matchedCtor == null)
            {
                throw new InvalidOperationException($"Ctor with parameters '{string.Join(", ", groupNames)}' not found.");
            }

            _constructor = matchedCtor;
        }

        public IEnumerable<T> Parse(string str)
        {
            var groupNames = _regex.GetGroupNames();
            var matches = GetMatches(str);
            foreach (var match in matches.Cast<Match>())
            {
                var dictionary = groupNames
                    .Select((name, idx) => new {name, idx})
                    .ToDictionary(s => s.name, s => match.Captures[s.idx].Value);

                var parameters = _constructor.Parameters
                    .Select(info =>
                    {
                        var value = dictionary[info.Name];
                        return Convert.ChangeType(value, info.ParameterType);
                    }).ToArray();

                var instance = _constructor.ConstructorInfo.Invoke(parameters);
                yield return (T) instance;
            }
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
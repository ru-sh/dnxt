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

    [NotNull]
    private readonly Initializer<T> _initializer;

    public RegexParser([NotNull]string regexPattern)
    {
      if (regexPattern == null) throw new ArgumentNullException(nameof(regexPattern));
      _regex = new Regex(regexPattern, RegexOptions.Compiled);
      _initializer = new Initializer<T>();
    }

    public IEnumerable<T> Parse(string str)
    {
      var matches = GetMatches(str);
      foreach (var match in matches)
      {
        var ctor = _initializer.Constructors
          .Select(constructor => new { constructor, parameters = constructor.Parameters.ToDictionary(info => info, info => match[info.Name]) })
          .Where(arg => arg.parameters.All(pair => IsFilled(pair.Key.ParameterType, match.ContainsKey(pair.Value))))
          .OrderByDescending(arg => arg.parameters.Count)
          .FirstOrDefault();

        if (ctor != null)
        {
          var parameters = ctor.parameters
            .Select(info => Convert.ChangeType(match[info.Name], info.ParameterType)).ToArray();

          var instance = (T)ctor.ConstructorInfo.Invoke(parameters);
          yield return instance;
        }
        else
        {
          throw new MatchConstructorNotFound(
            match.Keys.ToList(),
            _initializer.Constructors.Select(constructor => constructor.Parameters.Select(info => info.Name).ToList()));
        }
      }
    }

    private bool IsFilled(Type t, bool hasValue)
    {
      var isNullable = Nullable.GetUnderlyingType(t) == null;
      return isNullable || hasValue;
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
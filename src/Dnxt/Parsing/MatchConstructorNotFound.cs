using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Dnxt.Parsing
{
  public class MatchConstructorNotFound : Exception
  {
    [NotNull]
    public IReadOnlyCollection<string> Keys { get; }

    [NotNull]
    public IEnumerable<IReadOnlyCollection<string>> Ctors { get; }

    public MatchConstructorNotFound([NotNull] IReadOnlyCollection<string> keys,
      [NotNull] IEnumerable<IReadOnlyCollection<string>> ctors)
    {
      if (keys == null) throw new ArgumentNullException(nameof(keys));
      if (ctors == null) throw new ArgumentNullException(nameof(ctors));
      Keys = keys;
      Ctors = ctors;
    }

    public override string ToString()
    {
      return $"[{string.Join(", ", Keys)}] is matching for constructors:" + Environment.NewLine
             + string.Join(Environment.NewLine, Ctors.Select(c => string.Join((string) ", ", (IEnumerable<string>) c)));
    }
  }
}
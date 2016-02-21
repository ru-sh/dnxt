using System;

namespace Dnxt.Parsing
{
    public class MatchNotFoundException : Exception
    {
        public string SourceString { get; }
        public string SearchPattern { get; }

        public MatchNotFoundException(string source, string pattern)
        {
            SourceString = source;
            SearchPattern = pattern;
        }
    }
}
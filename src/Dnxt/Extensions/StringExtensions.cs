using JetBrains.Annotations;

namespace Dnxt.Extensions
{
    public static class StringExtensions
    {
        [CanBeNull]
        public static string ToPascalCase(this string s)
        {
            if (string.IsNullOrEmpty(s)) return s;

            var r = s.Substring(0, 1).ToUpperInvariant();
            if (s.Length > 1)
            {
                r = r + s.Substring(1);
            }

            return r;
        }
    }
}
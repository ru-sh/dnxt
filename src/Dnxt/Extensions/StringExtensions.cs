using System;
using JetBrains.Annotations;

namespace Dnxt.Extensions
{
    public static class StringExtensions
    {
        [CanBeNull]
        public static string ToPascalCase(this string s)
        {
            return ChangeFirstChar(s, s1 => s1.ToUpperInvariant());
        }

        [CanBeNull]
        public static string ToCamelCase(this string s)
        {
            return ChangeFirstChar(s, s1 => s1.ToLowerInvariant());
        }

        private static string ChangeFirstChar(string s, Func<string, string> func)
        {
            if (string.IsNullOrEmpty(s)) return s;

            var r = func(s.Substring(0, 1));
            if (s.Length > 1)
            {
                r = r + s.Substring(1);
            }

            return r;
        }
    }
}
using System;

namespace Dnxt
{
    public static class IntExtensions
    {
        public static TimeSpan Seconds(this int s)
        {
            return TimeSpan.FromSeconds(s);
        }
    }
}
using System;

namespace Dnxt.Env
{
    public class DefaultDateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
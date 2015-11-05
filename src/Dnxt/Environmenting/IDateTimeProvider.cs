using System;

namespace Dnxt.Env
{
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
    }

    public class DefaultDateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
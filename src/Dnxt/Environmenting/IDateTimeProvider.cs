using System;

namespace Dnxt.Environmenting
{
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
    }
}
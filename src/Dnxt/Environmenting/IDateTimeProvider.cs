using System;

namespace Dnxt.Env
{
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
    }
}
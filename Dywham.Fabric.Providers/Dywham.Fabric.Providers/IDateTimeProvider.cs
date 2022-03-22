using System;

namespace Dywham.Fabric.Providers
{
    public interface IDateTimeProvider : IProvider
    {
        DateTime ConvertFromNanoseconds(long nanoseconds, bool useUnixEpoch);

        DateTime GetUtcNow();

        DateTime GetNow();

        int GetIso8601WeekOfYear(DateTime time);
    }
}
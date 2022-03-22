using System;
using System.Globalization;

namespace Dywham.Fabric.Providers
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime ConvertFromNanoseconds(long nanoseconds, bool useUnixEpoch)
        {
            var refTime = useUnixEpoch ? new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) : DateTime.UtcNow;

            return refTime.AddTicks(nanoseconds / 100);
        }

        public DateTime GetUtcNow()
        {
            return DateTime.UtcNow;
        }

        public DateTime GetNow()
        {
            return DateTime.Now;
        }

        public int GetIso8601WeekOfYear(DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            var day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
    }
}

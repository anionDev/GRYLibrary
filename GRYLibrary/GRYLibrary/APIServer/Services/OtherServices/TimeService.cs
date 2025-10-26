using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.Misc;
using System;

namespace GRYLibrary.Core.APIServer.Services.OtherServices
{
    public class TimeService : ITimeService
    {
        public DateTimeOffset GetCurrentLocalTime()
        {
            return DateTimeOffset.Now;
        }

        public GRYDateTime GetCurrentLocalTimeAsGRYDateTime()
        {
            return GRYDateTime.FromDateTime(this.GetCurrentLocalTime());
        }

        public DateTimeOffset GetCurrentTimeInTimezone(TimeZoneInfo timeZone)
        {
            return TimeZoneInfo.ConvertTime(this.GetCurrentTimeInUTC(), timeZone);
        }

        public GRYDateTime GetCurrentTimeInTimezoneAsGRYDateTime(TimeZoneInfo timeZone)
        {
            return GRYDateTime.FromDateTime(this.GetCurrentTimeInTimezone(timeZone));
        }

        public DateTimeOffset GetCurrentTimeInUTC()
        {
            return DateTimeOffset.UtcNow;
        }

        public GRYDateTime GetCurrentTimeInUTCAsGRYDateTime()
        {
            return GRYDateTime.FromDateTime(this.GetCurrentTimeInUTC());
        }
    }
}

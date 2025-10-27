using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.Misc;
using System;

namespace GRYLibrary.Core.APIServer.Services.OtherServices
{
    public class TimeService : ITimeService
    {
        public DateTimeOffset GetCurrentLocalTimeAsDateTimeOffset()
        {
            return DateTimeOffset.Now;
        }

        public GRYDateTime GetCurrentLocalTimeAsGRYDateTime()
        {
            return GRYDateTime.FromDateTime(this.GetCurrentLocalTimeAsDateTimeOffset());
        }

        public DateTimeOffset GetCurrentTimeInTimezone(TimeZoneInfo timeZone)
        {
            return TimeZoneInfo.ConvertTime(this.GetCurrentTimeInUTCAsDateTimeOffset(), timeZone);
        }

        public GRYDateTime GetCurrentTimeInTimezoneAsGRYDateTime(TimeZoneInfo timeZone)
        {
            return GRYDateTime.FromDateTime(this.GetCurrentTimeInTimezone(timeZone));
        }

        public DateTimeOffset GetCurrentTimeInUTCAsDateTimeOffset()
        {
            return DateTimeOffset.UtcNow;
        }

        public GRYDateTime GetCurrentTimeInUTCAsGRYDateTime()
        {
            return GRYDateTime.FromDateTime(this.GetCurrentTimeInUTCAsDateTimeOffset());
        }
    }
}

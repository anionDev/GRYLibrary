using GRYLibrary.Core.Misc;
using System;

namespace GRYLibrary.Core.APIServer.Services.Interfaces
{
    public interface ITimeService
    {
        public DateTimeOffset GetCurrentLocalTime();
        public GRYDateTime GetCurrentLocalTimeAsGRYDateTime();
        public DateTimeOffset GetCurrentTimeInUTC();
        public GRYDateTime GetCurrentTimeInUTCAsGRYDateTime();
        public DateTimeOffset GetCurrentTimeInTimezone(TimeZoneInfo timeZone);
        public GRYDateTime GetCurrentTimeInTimezoneAsGRYDateTime(TimeZoneInfo timeZone);
    }
}

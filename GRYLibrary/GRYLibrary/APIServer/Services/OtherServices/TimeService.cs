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

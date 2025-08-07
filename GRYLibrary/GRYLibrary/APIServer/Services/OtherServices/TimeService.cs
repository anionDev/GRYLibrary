using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.Misc;
using System;
using GUtilities = GRYLibrary.Core.Misc.Utilities;

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

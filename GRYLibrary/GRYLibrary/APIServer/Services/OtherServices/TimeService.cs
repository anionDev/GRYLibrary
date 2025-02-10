using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.Misc;
using System;
using GUtilities = GRYLibrary.Core.Misc.Utilities;

namespace GRYLibrary.Core.APIServer.Services.OtherServices
{
    public class TimeService : ITimeService
    {
        public DateTime GetCurrentTime()
        {
            return GUtilities.GetNow();
        }

        public GRYDateTime GetCurrentTimeAsGRYDateTime()
        {
            return GRYDateTime.FromDateTime(this.GetCurrentTime());
        }

        public DateTime GetCurrentTimeInUTC()
        {
            return DateTime.UtcNow;
        }

        public GRYDateTime GetCurrentTimeInUTCAsGRYDateTime()
        {
            return GRYDateTime.FromDateTime(this.GetCurrentTimeInUTC());
        }
    }
}

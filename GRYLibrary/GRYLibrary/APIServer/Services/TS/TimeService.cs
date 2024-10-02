using GRYLibrary.Core.APIServer.Services.Interfaces;
using System;
using GUtilities = GRYLibrary.Core.Misc.Utilities;

namespace GRYLibrary.Core.APIServer.Services.TS
{
    public class TimeService : ITimeService
    {
        public DateTime GetCurrentTime()
        {
            return GUtilities.GetNow();
        }
    }
}

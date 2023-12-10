using GRYLibrary.Core.APIServer.Services.Interfaces;
using System;

namespace GRYLibrary.Core.APIServer.Services.TS
{
    public class TimeService : ITimeService
    {
        public DateTime GetCurrentTime()
        {
            return DateTime.Now;
        }
    }
}

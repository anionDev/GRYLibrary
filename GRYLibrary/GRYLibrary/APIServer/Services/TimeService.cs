using System;

namespace GRYLibrary.Core.APIServer.Services
{
    public class TimeService : ITimeService
    {
        public DateTime GetCurrentTime()
        {
            return DateTime.Now;
        }
    }
}

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
    }
}

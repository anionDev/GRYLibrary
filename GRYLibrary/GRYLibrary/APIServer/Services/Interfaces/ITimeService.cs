using GRYLibrary.Core.Misc;
using System;

namespace GRYLibrary.Core.APIServer.Services.Interfaces
{
    public interface ITimeService
    {
        public DateTimeOffset GetCurrentLocalTimeAsDateTimeOffset();
        public GRYDateTime GetCurrentLocalTimeAsGRYDateTime();
        public DateTimeOffset GetCurrentTimeInUTCAsDateTimeOffset();
        public GRYDateTime GetCurrentTimeInUTCAsGRYDateTime();
    }
}

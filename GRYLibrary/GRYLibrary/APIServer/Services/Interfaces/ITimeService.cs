using GRYLibrary.Core.Misc;
using System;

namespace GRYLibrary.Core.APIServer.Services.Interfaces
{
    public interface ITimeService
    {
        public DateTime GetCurrentTime();
        public GRYDateTime GetCurrentTimeAsGRYDateTime();
    }
}

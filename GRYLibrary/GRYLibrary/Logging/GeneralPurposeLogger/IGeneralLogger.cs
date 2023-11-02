using GRYLibrary.Core.Logging.GRYLogger;
using System;

namespace GRYLibrary.Core.Logging.GeneralPurposeLogger
{
    public interface IGeneralLogger
    {
        public Action<LogItem> AddLogEntry { get; set; }
    }
}
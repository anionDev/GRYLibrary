using GRYLibrary.Core.Log;
using System;

namespace GRYLibrary.Core.GeneralPurposeLogger
{
    public interface IGeneralLogger
    {
        public Action<LogItem> AddLogEntry { get; set; }
    }
}
using GRYLibrary.Core.Log;
using System;

namespace GRYLibrary.Core.GenericWebAPIServer.Services
{
    public interface IGeneralLogger
    {
        public Action<LogItem> AddLogEntry { get; set; }      
    }
}
﻿using GRYLibrary.Core.APIServer.MidT.RLog;
using GRYLibrary.Core.Logging.GRYLogger;
using System.Collections.Generic;

namespace GRYLibrary.Core.APIServer.Mid.DLog
{
    public interface IDRequestLoggingConfiguration : IRequestLoggingConfiguration
    {
        public bool AddMillisecondsInLogTimestamps { get; set; }
        public bool LogClientIP { get; set; }
        public GRYLogConfiguration RequestsLogConfiguration { get; set; }
        public uint MaximalLengthofRequestBodies { get; set; }
        public uint MaximalLengthofResponseBodies { get; set; }
        public ISet<string> NotLoggedRoutes { get; set; }
    }
}
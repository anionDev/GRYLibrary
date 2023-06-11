using GRYLibrary.Core.GenericWebAPIServer.Middlewares;
using GRYLibrary.Core.Log;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;

namespace GRYLibrary.Core.GenericWebAPIServer.DefinedMiddlewares.RequestLogging
{
    public interface IRequestLoggingConfiguration :IMiddlewareConfiguration
    {
        public bool AddMillisecondsInLogTimestamps { get; set; }
        public bool LogClientIP { get; set; }
        public GRYLogConfiguration RequestsLogConfiguration { get; set; }
        public uint MaximalLengthofBodies { get; set; }
    }
}
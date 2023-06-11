using GRYLibrary.Core.Log;
using GRYLibrary.Core.Miscellaneous.FilePath;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Net;

namespace GRYLibrary.Core.GenericWebAPIServer.DefinedMiddlewares.RequestLogging
{
    public class RequestLoggingConfiguration :IRequestLoggingConfiguration
    {
        public bool AddMillisecondsInLogTimestamps { get; set; } = false;
        public GRYLogConfiguration RequestsLogConfiguration { get; set; } =  GRYLogConfiguration.GetCommonConfiguration(RelativeFilePath.FromString("./Requests.log"));
        public bool Enabled { get; set; } = true;
        public bool LogClientIP { get; set; } = true;
        public uint MaximalLengthofBodies { get; set; }


        public ISet<FilterDescriptor> GetFilter()
        {
            return new HashSet<FilterDescriptor>();
        }
    }
}
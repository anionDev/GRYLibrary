using GRYLibrary.Core.Log;
using GRYLibrary.Core.Miscellaneous.FilePath;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

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
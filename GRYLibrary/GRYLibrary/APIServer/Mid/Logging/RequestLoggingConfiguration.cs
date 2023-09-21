using GRYLibrary.Core.Log;
using GRYLibrary.Core.Miscellaneous.FilePath;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace GRYLibrary.Core.APIServer.Mid.Logging
{
    public class RequestLoggingConfiguration : IRequestLoggingConfiguration
    {
        public bool AddMillisecondsInLogTimestamps { get; set; } = false;
        public GRYLogConfiguration RequestsLogConfiguration { get; set; } = GRYLogConfiguration.GetCommonConfiguration(AbstractFilePath.FromString("./Requests.log"));
        public bool Enabled { get; set; } = true;
        public bool LogClientIP { get; set; } = true;
        public uint MaximalLengthofBodies { get; set; }


        public ISet<FilterDescriptor> GetFilter()
        {
            return new HashSet<FilterDescriptor>();
        }
    }
}
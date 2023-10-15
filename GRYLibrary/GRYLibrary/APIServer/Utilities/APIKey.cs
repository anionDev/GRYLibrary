using System;

namespace GRYLibrary.Core.APIServer.Utilities
{
    public class APIKey
    {
        public string APIKeyValue { get; set; }
        public DateTime ExpireMoment { get; set; }
        public string Description { get; set; }
    }
}

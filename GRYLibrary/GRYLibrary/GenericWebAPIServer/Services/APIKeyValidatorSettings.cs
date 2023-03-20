using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.Services
{
    public class APIKeyValidatorSettings : IAPIKeyValidatorSettings
    {
        public bool Enabled { get; set; } = false;

        public bool AnonymousAccessIsAllowed(string route)
        {
            return true;//TODO
        }

        public bool APIKeyIsValid(string apiKey, string route)
        {
            return true;//TODO
        }
    }
}

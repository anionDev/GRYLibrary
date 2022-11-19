using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    public class ServerSettings
    {
        public ulong RequestsPerMinuteLimit { get; set; }
        public ulong MaxRequestBodySize { get; set; }
    }
}

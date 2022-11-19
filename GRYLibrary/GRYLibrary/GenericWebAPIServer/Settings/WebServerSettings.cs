using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    public class WebServerSettings
    {
        public string Domain { get; set; }
        public ushort Port { get; set; }
        public EncryptionSettings EncryptionSettings { get; set; }

    }
}

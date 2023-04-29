using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.v2.Settings
{
    public class WebServerConfiguration
    {
        public string Domain { get; set; }
        public bool UseHTTPS { get; set; }
        public ushort Port { get; set; }
        public string CertificatePasswordFile { get; set; }
        public string CertificatePFXFile { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    public class EncryptionSettings
    {
        public string CertificateFile { get; set; }
        public string CertificatePasswordFile { get; set; }
    }
}

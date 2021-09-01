using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer
{
    public interface ISettingsInterface
    {
        public bool ProgramVersionIsQueryable{ get; set; }
        public ushort HTTPSPort { get; set; }
        public string CertificateFile { get; set; }
        public string CertificatePassword { get; set; }
        public long MaxRequestBodySize { get; set; }
    }
}

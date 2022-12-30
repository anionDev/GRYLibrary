using GRYLibrary.Core.XMLSerializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    public class ApplicationSettings
    {
        public string TermsOfServiceURL { get; set; }
        public string ContactURL { get; set; }
        public string LicenseURL { get; set; }
        public string AppDescription { get; set; }
        public SerializableDictionary<string, object> CustomConfigurations { get; set; } = new SerializableDictionary<string, object>();
    }
}

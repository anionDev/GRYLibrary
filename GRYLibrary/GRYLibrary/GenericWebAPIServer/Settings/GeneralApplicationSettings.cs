using GRYLibrary.Core.Log;

namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    public class GeneralApplicationSettings
    {
        public bool Enabled { get; set; }
        public string TermsOfServiceURL { get; set; }
        public string ContactURL { get; set; }
        public string LicenseURL { get; set; }
        public string AppDescription { get; set; }
        public GRYLogConfiguration LogConfiguration { get; set; }
    }
}
using GRYLibrary.Core.Log;

namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    /// <summary>
    /// Represents general configuration-settings for an application which are allowed to be changed in a configuration-file.
    /// </summary>
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
using System.IO;
using System.Runtime.InteropServices;

namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    public class ApplicationSettings
    {
        public ApplicationSettings()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                LogFolder = Path.Join(Directory.GetCurrentDirectory(), "Logs");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                || RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                || RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            {
                LogFolder = "/Workspace/Logs";
            }
            else
            {
                throw new System.Exception("The current OS is not supported.");
            }
        }
        public string TermsOfServiceURL { get; set; }
        public string ContactURL { get; set; }
        public string LicenseURL { get; set; }
        public string AppDescription { get; set; }
        public string LogFolder { get; set; }
    }
}

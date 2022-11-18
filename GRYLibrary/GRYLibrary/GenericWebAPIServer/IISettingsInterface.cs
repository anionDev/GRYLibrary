namespace GRYLibrary.Core.GenericWebAPIServer
{
    public interface IISettingsInterface
    {
        public string Domain { get; set; }
        public ushort HTTPSPort { get; set; }
        public string CertificateFile { get; set; }
        public string CertificatePasswordFile { get; set; }

        public string GetLogFolder();
        public System.Version GetVersion();
        public IEnvironment GetEnvironment();
        public string GetProgramName();
    }
}

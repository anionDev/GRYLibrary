namespace GRYLibrary.Core.GenericWebAPIServer
{
    public interface ISettingsInterface
    {
        public string Domain { get; set; }
        public ushort HTTPSPort { get; set; }
        public string CertificateFile { get; set; }
        public string CertificatePasswordFile { get; set; }
        public long MaxRequestBodySize { get; set; }
    }
}

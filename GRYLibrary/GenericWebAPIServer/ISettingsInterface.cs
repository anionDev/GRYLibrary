namespace GRYLibrary.Core.GenericWebAPIServer
{
    public interface ISettingsInterface
    {
        public ushort HTTPSPort { get; set; }
        public string CertificateFile { get; set; }
        public string CertificatePasswordFile { get; set; }
        public bool ProgramVersionIsQueryable{ get; set; }
        public long MaxRequestBodySize { get; set; }
    }
}

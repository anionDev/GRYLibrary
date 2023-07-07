using GRYLibrary.Core.Miscellaneous.FilePath;

namespace GRYLibrary.Core.GenericWebAPIServer.Settings.Configuration
{
    public class TLSCertificateInformation
    {
        public AbstractFilePath CertificatePFXFile { get; set; } = default;
        public AbstractFilePath CertificatePasswordFile { get; set; } = default;
        public string FallbackCertificatePasswordFileContentHex { get; set; } = default;
        public string FallbackCertificatePFXFileContentHex { get; set; } = default;
        public static TLSCertificateInformation Create(AbstractFilePath certificatePFXFile, AbstractFilePath certificatePasswordFile, string fallbackCertificatePasswordFileContentHex, string fallbackCertificatePFXFileContentHex)
        {
            TLSCertificateInformation result = new TLSCertificateInformation
            {
                CertificatePFXFile = certificatePFXFile,
                CertificatePasswordFile = certificatePasswordFile,
                FallbackCertificatePasswordFileContentHex = fallbackCertificatePasswordFileContentHex,
                FallbackCertificatePFXFileContentHex = fallbackCertificatePFXFileContentHex
            };
            return result;
        }
        public string GetCertificatePasswordFile(string certificateFolder) { return this.CertificatePasswordFile.GetPath(certificateFolder); }
        public string GetCertificatePFXFile(string certificateFolder) { return this.CertificatePFXFile.GetPath(certificateFolder); }
    }
}

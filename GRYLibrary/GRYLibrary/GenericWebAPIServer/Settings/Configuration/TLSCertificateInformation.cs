using GRYLibrary.Core.Miscellaneous.FilePath;

namespace GRYLibrary.Core.GenericWebAPIServer.Settings.Configuration
{
    public class TLSCertificateInformation
    {
        public AbstractFilePath CertificatePasswordFile { get; set; } = default;
        public AbstractFilePath CertificatePFXFile { get; set; } = default;
        public string FallbackCertificatePasswordFileContentHex { get; set; } = default;
        public string FallbackCertificatePFXFileContentHex { get; set; } = default;
        public static TLSCertificateInformation Create(AbstractFilePath certificatePasswordFile, AbstractFilePath certificatePFXFile, string fallbackCertificatePasswordFileContentHex, string fallbackCertificatePFXFileContentHex)
        {
            TLSCertificateInformation result = new TLSCertificateInformation
            {
                CertificatePasswordFile = certificatePasswordFile,
                CertificatePFXFile = certificatePFXFile,
                FallbackCertificatePasswordFileContentHex = fallbackCertificatePasswordFileContentHex,
                FallbackCertificatePFXFileContentHex = fallbackCertificatePFXFileContentHex
            };
            return result;
        }
        public string GetCertificatePasswordFile(string certificateFolder) { return this.CertificatePasswordFile.GetPath(certificateFolder); }
        public string GetCertificatePFXFile(string certificateFolder) { return this.CertificatePFXFile.GetPath(certificateFolder); }
    }
}

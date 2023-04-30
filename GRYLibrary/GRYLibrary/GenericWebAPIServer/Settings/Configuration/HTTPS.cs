namespace GRYLibrary.Core.GenericWebAPIServer.Settings.Configuration
{
    public class HTTPS :Protocol
    {
        public TLSCertificateInformation TLSCertificateInformation { get; set; }
        public static HTTPS Create(TLSCertificateInformation tlsCertificateInformation)
        {
            HTTPS result = new HTTPS();
            result.Port = 443;
            result.TLSCertificateInformation = tlsCertificateInformation;
            return result;
        }
        public override string GetProtocol()
        {
            return "https";
        }
    }
}

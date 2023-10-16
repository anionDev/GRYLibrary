namespace GRYLibrary.Core.APIServer.Settings.Configuration
{
    public class HTTPS : Protocol
    {
        public const ushort DefaultPort = 443;
        public TLSCertificateInformation TLSCertificateInformation { get; set; }
        public static HTTPS Create(TLSCertificateInformation tlsCertificateInformation)
        {
            HTTPS result = new HTTPS
            {
                Port = DefaultPort,
                TLSCertificateInformation = tlsCertificateInformation
            };
            return result;
        }
        public override string GetProtocol()
        {
            return "https";
        }
    }
}

using GRYLibrary.Core.APIServer.ExecutionModes;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.Miscellaneous.FilePath;

namespace GRYLibrary.Core.APIServer.Utilities
{
    public class GetProcolVisitor : IExecutionModeVisitor<Protocol>
    {
        private readonly HTTP _HTTP;
        private readonly string _Domain;

        public GetProcolVisitor(string domain)
        {
            this._Domain = domain;
            this._HTTP = HTTP.Create();
        }
        public GetProcolVisitor(string domain, ushort httpPort)
        {
            this._Domain = domain;
            this._HTTP = HTTP.Create(httpPort);
        }

        public Protocol Handle(Analysis analysis)
        {
            return this._HTTP;
        }

        public Protocol Handle(RunProgram runProgram)
        {
            return HTTPS.Create(new TLSCertificateInformation
            {
                CertificatePFXFile = AbstractFilePath.FromString($"./{this._Domain}.pfx"),
                CertificatePasswordFile = AbstractFilePath.FromString($"./{this._Domain}.password"),
            });
        }

        public Protocol Handle(TestRun testRun)
        {
            return this._HTTP;
        }
    }
}

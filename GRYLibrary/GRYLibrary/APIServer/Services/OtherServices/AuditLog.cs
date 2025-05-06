using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.Logging.GRYLogger;

namespace GRYLibrary.Core.APIServer.Services.OtherServices
{
    public class AuditLog : IAuditLog
    {
        public IGRYLog AuditLogger { get; private set; }
        public AuditLog(IGRYLog log)
        {
            this.AuditLogger = log;
        }
    }
}

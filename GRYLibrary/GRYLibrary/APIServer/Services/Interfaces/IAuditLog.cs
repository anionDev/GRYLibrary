using GRYLibrary.Core.Logging.GRYLogger;

namespace GRYLibrary.Core.APIServer.Services.Interfaces
{
    public interface IAuditLog
    {
        public IGRYLog AuditLogger { get; }
    }
}

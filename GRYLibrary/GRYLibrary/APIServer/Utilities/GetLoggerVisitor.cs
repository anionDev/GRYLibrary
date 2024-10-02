using GRYLibrary.Core.APIServer.ExecutionModes;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using GRYLibrary.Core.Logging.GRYLogger;

namespace GRYLibrary.Core.APIServer.Utilities
{

    public class GetLoggerVisitor : IExecutionModeVisitor<IGRYLog>
    {
        private readonly GRYLogConfiguration _LogConfiguration;
        private readonly string _BaseFolder;
        private readonly string _LoggerName;
        public GetLoggerVisitor(GRYLogConfiguration logConfiguration, string baseFolder, string loggerName)
        {
            this._LogConfiguration = logConfiguration;
            this._BaseFolder = baseFolder;
            this._LoggerName = loggerName;
        }
        public IGRYLog Handle(Analysis analysis)
        {
            return GeneralLogger.NoLogAsGRYLog();// avoid creation of logging-entries when doing something like generate APISpecification-artifact by running "swagger tofile ..."
        }

        public IGRYLog Handle(RunProgram runProgram)
        {
            return this.GetUsualLog();
        }

        public IGRYLog Handle(TestRun testRun)
        {
            return this.GetUsualLog();
        }

        private IGRYLog GetUsualLog()
        {
            IGRYLog result = GeneralLogger.CreateUsingGRYLog(this._LogConfiguration, this._BaseFolder);
            result.BasePath = this._BaseFolder;
            result.UseSubNamespace(this._LoggerName);
            return result;
        }
    }
}

using GRYLibrary.Core.GeneralPurposeLogger;
using GRYLibrary.Core.APIServer.ExecutionModes;
using GRYLibrary.Core.Log;

namespace GRYLibrary.Core.APIServer.Utilities
{

    public class GetLoggerVisitor :IExecutionModeVisitor<IGeneralLogger>
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
        public IGeneralLogger Handle(Analysis analysis)
        {
            return GeneralLogger.NoLog();// avoid creation of logging-entries when doing something like generate APISpecification-artifact by running "swagger tofile ..."
        }

        public IGeneralLogger Handle(RunProgram runProgram)
        {
            GeneralLogger result = GeneralLogger.CreateUsingGRYLog(this._LogConfiguration, out GRYLog logger, this._BaseFolder);
            logger.BasePath = this._BaseFolder;
            logger.UseSubNamespace(this._LoggerName);
            return result;
        }
    }
}

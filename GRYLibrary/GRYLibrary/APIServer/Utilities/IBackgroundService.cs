using GRYLibrary.Core.APIServer.ExecutionModes;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;

namespace GRYLibrary.Core.APIServer.Utilities
{
    public interface IBackgroundService
    {
        public bool Enabled { get; }
        public IGeneralLogger Logger { get; }
        public abstract void StartAsyncImplementation();
        public abstract void StopImplementation();
        public void StartAsync(ExecutionMode executionMode)
        {
            if (this.ShouldBeExecuted(executionMode))
            {
                this.Logger.Log($"Background-service {this.GetType().Name} will be started.", Microsoft.Extensions.Logging.LogLevel.Information);
                this.StartAsyncImplementation();
            }
            else
            {
                this.Logger.Log($"Background-service {this.GetType().Name} will not be started.", Microsoft.Extensions.Logging.LogLevel.Information);
            }
        }
        public void Stop(ExecutionMode executionMode)
        {
            if (this.ShouldBeExecuted(executionMode))
            {
                this.Logger.Log($"Background-service {this.GetType().Name} will be stopped.", Microsoft.Extensions.Logging.LogLevel.Information);
                this.StartAsyncImplementation();
            }
        }
        public bool ShouldBeExecuted(ExecutionMode executionMode)
        {
            if (executionMode is Analysis)
            {
                return false;
            }
            if (!this.Enabled)
            {
                return false;
            }
            return true;
        }
    }
}

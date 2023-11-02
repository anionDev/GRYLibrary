using GRYLibrary.Core.APIServer.ExecutionModes;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using GUtilities = GRYLibrary.Core.Miscellaneous.Utilities;
using System.Threading.Tasks;
using System.Threading;

namespace GRYLibrary.Core.APIServer.Utilities
{
    public abstract class IteratingBackgroundService
    {
        public bool Enabled { get; set; }
        protected bool Running { get; private set; }
        public IGeneralLogger Logger { get; protected set; }
        private readonly ExecutionMode _ExecutionMode;
        protected abstract void Run();
        public IteratingBackgroundService(ExecutionMode executionMode)
        {
            this._ExecutionMode = executionMode;
        }
        public void StartAsync()
        {
            if (!this.Running)
            {
                this.Running = true;
                if (this.ShouldBeExecuted())
                {
                    this.Logger.Log($"Background-service {this.GetType().Name} will be started.", Microsoft.Extensions.Logging.LogLevel.Debug);
                    Task.Run(() =>
                    {
                        while (this.Enabled)
                        {
                            Logger.Log($"Execute {this.GetType().Name}", Microsoft.Extensions.Logging.LogLevel.Debug, false, false, false, false, this.Run);
                            Thread.Sleep(50);
                        }
                    });
                }
                else
                {
                    this.Logger.Log($"Background-service {this.GetType().Name} will not be started.", Microsoft.Extensions.Logging.LogLevel.Debug);
                }
            }
        }
        public async Task Stop()
        {
            if (this.Running)
            {
                this.Logger.Log($"Background-service {this.GetType().Name} will be stopped.", Microsoft.Extensions.Logging.LogLevel.Debug);
                this.Enabled = false;
                await GUtilities.WaitUntilConditionIsTrueAsync(() => !this.Running);
                this.Logger.Log($"Background-service {this.GetType().Name} stopped.", Microsoft.Extensions.Logging.LogLevel.Debug);
            }
        }
        public bool ShouldBeExecuted()
        {
            if (this._ExecutionMode is not RunProgram)
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

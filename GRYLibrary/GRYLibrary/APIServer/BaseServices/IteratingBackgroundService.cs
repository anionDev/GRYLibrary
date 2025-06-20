using GRYLibrary.Core.APIServer.ExecutionModes;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using GUtilities = GRYLibrary.Core.Misc.Utilities;
using System.Threading.Tasks;
using System.Threading;
using System;
using GRYLibrary.Core.Logging.GRYLogger;
using Microsoft.Extensions.Logging;

namespace GRYLibrary.Core.APIServer.BaseServices
{
    public abstract class IteratingBackgroundService : IDisposable
    {
        public bool Enabled { get; set; }
        protected bool Running { get; private set; }
        public TimeSpan AdditionalDelay { get; set; } = TimeSpan.FromSeconds(0);
        protected readonly IGRYLog _Logger;
        private readonly ExecutionMode _ExecutionMode;
        protected abstract void Run();
        public IteratingBackgroundService(ExecutionMode executionMode, IGRYLog logger)
        {
            this._ExecutionMode = executionMode;
            this._Logger = logger;
        }
        public void StartAsync()
        {
            if (!this.Running)
            {
                this.Running = true;
                if (this.ShouldBeExecuted())
                {
                    this._Logger.Log($"Background-service {this.GetType().Name} will be started.", LogLevel.Information);
                    Task.Run(() =>
                    {
                        while (this.Enabled)
                        {
                            Thread.Sleep(50);
                            Thread.Sleep(this.AdditionalDelay);
                            this._Logger.Log($"Execute {this.GetType().Name}", LogLevel.Debug, false, false, true, false, false, this.Run);
                        }
                    });
                }
                else
                {
                    this._Logger.Log($"Background-service {this.GetType().Name} will not be started.", LogLevel.Information);
                }
            }
        }
        public async Task Stop()
        {
            if (this.Running)
            {
                this._Logger.Log($"Background-service {this.GetType().Name} will be stopped.", LogLevel.Information);
                this.Enabled = false;
                await GUtilities.WaitUntilConditionIsTrueAsync(() => !this.Running);
                this.Dispose();
                this._Logger.Log($"Background-service {this.GetType().Name} is now stopped.", LogLevel.Information);
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

        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}

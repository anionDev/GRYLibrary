using GRYLibrary.Core.APIServer.ExecutionModes;

namespace GRYLibrary.Core.APIServer.Utilities
{
    public interface IBackgroundService
    {
        public bool Enabled { get; }
        public abstract void StartAsyncImplementation();
        public abstract void StopImplementation();
        public void StartAsync(ExecutionMode executionMode)
        {
            if (this.ShouldBeExecuted(executionMode))
            {
                this.StartAsyncImplementation();
            }
        }
        public void Stop(ExecutionMode executionMode)
        {
            if (this.ShouldBeExecuted(executionMode))
            {
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

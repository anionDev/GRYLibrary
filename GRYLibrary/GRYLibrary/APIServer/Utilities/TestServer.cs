using System.Diagnostics;

namespace GRYLibrary.Core.APIServer.Utilities
{
    public sealed class TestServer : System.IDisposable
    {
        public string APIKey { get; private set; }
        public string ProgramFile { get; private set; }
        private readonly Process _Process;
        public TestServer(string programFile, string apiKey)
        {
            this.ProgramFile = programFile;
            this.APIKey = apiKey;


            var process = new Process
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = ProgramFile,
                    Arguments = "--TestRun"
                },
                EnableRaisingEvents = true
            };
            this._Process = process;

            process.Start();
        }
        public void Dispose()
        {
            this._Process.Kill();
            this._Process.Dispose();
        }
    }
}

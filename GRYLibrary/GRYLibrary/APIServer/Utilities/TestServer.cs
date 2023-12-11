using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace GRYLibrary.Core.APIServer.Utilities
{
    public sealed class TestServer : IDisposable
    {
        public static readonly TimeSpan DefaultWaitTimeAfterProcessStart = TimeSpan.FromSeconds(2);
        public string APIKey { get; private set; }
        public string ProgramFile { get; private set; }
        private readonly Process _Process;
        public TestServer(string programFile, string apiKey) : this(programFile, apiKey, DefaultWaitTimeAfterProcessStart)
        {
        }
        public TestServer(string programFile, string apiKey, TimeSpan waitTimeAfterProcessStart)
        {
            this.ProgramFile = programFile;
            this.APIKey = apiKey;

            Process process = new Process
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = this.ProgramFile,
                    Arguments = "--TestRun",
                    WorkingDirectory = Path.GetDirectoryName(this.ProgramFile),
                },
                EnableRaisingEvents = true
            };
            this._Process = process;
            process.Start();
            Thread.Sleep(waitTimeAfterProcessStart);
        }

        public void Dispose()
        {
            this._Process.Kill();
            this._Process.Dispose();
        }
    }
}

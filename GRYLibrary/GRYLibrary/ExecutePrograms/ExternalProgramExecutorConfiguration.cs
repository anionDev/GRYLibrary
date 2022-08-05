﻿using GRYLibrary.Core.ExecutePrograms.WaitingStates;

namespace GRYLibrary.Core.ExecutePrograms
{
    public class ExternalProgramExecutorConfiguration
    {
        public string Program { get; set; }

        public string Argument { get; set; } = string.Empty;
        public bool ArgumentIsBase64Encoded { get; set; } = false;
        public string WorkingDirectory { get; set; } = null;
        /// <remarks>
        /// Default-value: <see cref="Verbosity.Quiet"/>
        /// </remarks>
        public Verbosity Verbosity { get; set; } = Verbosity.Quiet;
        public bool PrintErrorsAsInformation { get; set; } = false;
        public bool AddLogOverhead { get; set; } = false;
        public string LogFile { get; set; } = null;
        public string StdOutFile { get; set; } = null;
        public string StdErrFile { get; set; } = null;
        public string ExitCodeFile { get; set; } = null;
        public string ProcessIdFile { get; set; } = null;
        public int? TimeoutInMilliseconds { get; set; } = null;
        public string Title { get; set; } = null;
        public WaitingState WaitingState { get; set; } = new RunSynchronously();
        public bool CreateWindow { get; set; } = true;

        public string LogNamespace { get; set; } = null;

        public string User { get; set; } = null;
        public string Password{ get; set; } = null;
        public bool UpdateConsoleTitle { get; set; } = false;
        public bool DelegateToEpew { get; set; } = false;
    }
}

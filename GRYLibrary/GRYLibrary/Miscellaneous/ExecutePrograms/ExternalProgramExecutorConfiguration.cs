using GRYLibrary.Core.Miscellaneous.ExecutePrograms.WaitingStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.Miscellaneous.ExecutePrograms
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

        public bool ElevatePrivileges { get; set; } = false;
        public bool UpdateConsoleTitle { get; set; } = false;
        public bool DelegateToEpew { get; set; } = false;
    }
}

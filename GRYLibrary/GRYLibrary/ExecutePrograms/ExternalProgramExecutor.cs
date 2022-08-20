﻿using GRYLibrary.Core.Exceptions;
using GRYLibrary.Core.Log;
using GRYLibrary.Core.ExecutePrograms.WaitingStates;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GRYLibrary.Core.Miscellaneous;

namespace GRYLibrary.Core.ExecutePrograms
{
    public sealed class ExternalProgramExecutor : IDisposable
    {
        public ExternalProgramExecutor(string programPathAndFile) : this(programPathAndFile, Utilities.EmptyString, null)
        {
        }
        public ExternalProgramExecutor(string programPathAndFile, string arguments) : this(programPathAndFile, arguments, null)
        {
        }
        public ExternalProgramExecutor(string program, string argument, string workingDirectory) : this(new ExternalProgramExecutorConfiguration()
        {
            Program = program,
            Argument = argument,
            WorkingDirectory = workingDirectory
        })
        {
        }
        public ExternalProgramExecutor(ExternalProgramExecutorConfiguration configuration)
        {
            this.Configuration = configuration;
        }
        public ExternalProgramExecutorConfiguration Configuration { get; }
        public ExecutionState CurrentExecutionState { get; private set; } = ExecutionState.NotStarted;
        public GRYLog LogObject { get; set; }
        internal string CMD { get; private set; }
        public delegate void ExecutionFinishedHandler(ExternalProgramExecutor sender, int exitCode);
        public event ExecutionFinishedHandler ExecutionFinishedEvent;
        private bool _Running = false;
        private TimeSpan _ExecutionDuration = default;
        public TimeSpan ExecutionDuration
        {
            get
            {
                if (this.CurrentExecutionState == ExecutionState.Terminated)
                {
                    return this._ExecutionDuration;
                }
                else
                {
                    throw new InvalidOperationException(this.GetInvalidOperationDueToNotTerminatedMessageByMembername(nameof(this.ExecutionDuration), ExecutionState.Terminated, true));
                }
            }
            private set { this._ExecutionDuration = value; }
        }
        public bool IsRunning
        {
            get
            {
                return this._Running;
            }
        }
        private readonly object _LockObject = new();
        private readonly ConcurrentQueue<(LogLevel, string)> _NotLoggedOutputLines = new();
        public void Run()
        {

            Configuration.WaitingState.Accept(new WaitingStateRunVisitor(this));
        }
        private class WaitingStateRunVisitor : IWaitingStateVisitor
        {
            private readonly ExternalProgramExecutor _ExternalProgramExecutor;

            public WaitingStateRunVisitor(ExternalProgramExecutor externalProgramExecutor)
            {
                this._ExternalProgramExecutor = externalProgramExecutor;
            }

            public void Handle(RunAsynchronously runAsynchronously)
            {
                _ExternalProgramExecutor.StartAsynchronously();
            }

            public void Handle(RunSynchronously runSynchronously)
            {
                _ExternalProgramExecutor.StartSynchronously();
            }
        }
        private Task StartAsynchronously()
        {
            this.Prepare();
            return this.StartProgram();
        }
        /// <summary>
        /// Starts the program which was set in the properties.
        /// </summary>
        /// <returns>
        /// The exit-code of the executed program will be returned.
        /// </returns>
        /// <exception cref="UnexpectedExitCodeException">
        /// Will be thrown if <see cref="RunSynchronously.ThrowErrorIfExitCodeIsNotZero"/> and the exitcode of the executed program is not 0.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Will be thrown if <see cref="StartSynchronously"/> was already called.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Will be thrown if the given working-directory does not exist.
        /// </exception>
        /// <exception cref="ProcessStartException">
        /// Will be thrown if the process could not be started.
        /// </exception>
        private int StartSynchronously()
        {
            var task = StartAsynchronously();
            task.Wait();
            return ExitCode;
        }
        private void Prepare()
        {
            this.CheckIfStartOperationWasAlreadyCalled();
            if (string.IsNullOrWhiteSpace(this.Configuration.LogNamespace))
            {
                this.Configuration.LogNamespace = string.Empty;
            }
            if (this.LogObject == default)
            {
                this.LogObject = GRYLog.Create();
                if (this.Configuration.Verbosity == Verbosity.Verbose)
                {
                    foreach (GRYLogTarget logtarget in this.LogObject.Configuration.LogTargets)
                    {
                        logtarget.LogLevels.Add(LogLevel.Debug);
                    }
                }
            }
            this.ResolvePaths();
            this.CMD = $"{this.Configuration.WorkingDirectory}>{this.Configuration.Program} {this.Configuration.Argument}";
            if (this.Configuration.Title == null)
            {
                this.Configuration.Title = string.Empty;
            }
            this.LogStart();
        }

        public static string CreateEpewArgumentString(string programPathAndFile, string arguments, string workingDirectory, bool printErrorsAsInformation, int? timeoutInMilliseconds, Verbosity verbosity, bool addLogOverhead, string logFile, string title, WaitingState waitingState, string logNamespace, string user, string password)
        {
            string result = $"--Program \"{programPathAndFile}\"";
            if (arguments != null)
            {
                result = $"{result} --Argument {Convert.ToBase64String(new System.Text.UTF8Encoding(false).GetBytes(arguments))} ";
                result = $"{result} --ArgumentIsBase64Encoded";
            }
            if (workingDirectory != null)
            {
                result = $"{result} --Workingdirectory \"{workingDirectory}\"";
            }
            if (timeoutInMilliseconds.HasValue)
            {
                result = $"{result} --TimeoutInMilliseconds \"{timeoutInMilliseconds}\"";
            }
            result = $"{result} --Verbosity {(int)verbosity}";
            if (printErrorsAsInformation)
            {
                result = $"{result} --PrintErrorsAsInformation";
            }
            if (addLogOverhead)
            {
                result = $"{result} --AddLogOverhead";
            }
            if (logFile != null)
            {
                result = $"{result} --Logfile \"{logFile}\"";
            }
            if (title != null)
            {
                result = $"{result} --Title \"{title}\"";
            }
            //TODO handle waiting-state
            if (logNamespace != null)
            {
                result = $"{result} --LogNamespace \"{logNamespace}\"";
            }
            if (user!=null)
            {
                result = $"{result} --User \"{user}\" --Password \"{password}\"";
            }
            result = $"{result} {waitingState.Accept(_GetWaitingStateCreateEpewArgumentStringVisitor)}";
            // TODO add missing Epew-arguments
            return result;
        }
        private static readonly IWaitingStateVisitor<string> _GetWaitingStateCreateEpewArgumentStringVisitor = new GetWaitingStateCreateEpewArgumentStringVisitor();
        private class GetWaitingStateCreateEpewArgumentStringVisitor : IWaitingStateVisitor<string>
        {
            public string Handle(RunAsynchronously runAsynchronously)
            {
                return "--NotSynchronous";
            }

            public string Handle(RunSynchronously runSynchronously)
            {
                if (runSynchronously.ThrowErrorIfExitCodeIsNotZero)
                {
                    return "--ThrowErrorIfExitCodeIsNotZero";
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <returns>
        /// Returns a summary of the executed program with its error-code, console-outputs, etc.
        /// </returns>
        /// <remarks>
        /// This summary is designed for readability and not for a further program-controlled processing of the data. For that purpose please read out the properties of this object.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// If the process is not terminated yet.
        /// </exception>
        public string GetSummaryOfExecutedProgram(bool includeStdOutAndStdErr = false)
        {
            if (this.CurrentExecutionState == ExecutionState.Terminated)
            {
                string result = $"{nameof(ExternalProgramExecutor)}-summary:";
                result = result + Environment.NewLine + $"Title: {this.Configuration.Title}";
                result = result + Environment.NewLine + $"Executed program: {this.CMD}";
                result = result + Environment.NewLine + $"Process-Id: {this.ProcessId}";
                result = result + Environment.NewLine + $"Exit-code: {this.ExitCode}";
                result = result + Environment.NewLine + $"Execution-duration: {this.ExecutionDuration:d'd 'h'h 'm'm 's's'}";
                if (includeStdOutAndStdErr)
                {
                    result = result + Environment.NewLine + $"StdOut:" + Environment.NewLine + string.Join(Environment.NewLine + "    ", this.AllStdOutLines);
                    result = result + Environment.NewLine + $"StdErr:" + Environment.NewLine + string.Join(Environment.NewLine + "    ", this.AllStdErrLines);
                }
                return result;
            }
            else
            {
                throw new InvalidOperationException(this.GetInvalidOperationDueToNotTerminatedMessageByMembername(nameof(this.GetSummaryOfExecutedProgram), ExecutionState.Terminated, true));
            }
        }

        private void LogStart()
        {
            if (string.IsNullOrWhiteSpace(this.Configuration.Title))
            {
                this.LogObject.Log($"Start executing program", LogLevel.Debug);
            }
            else
            {
                this.LogObject.Log($"Start executing '{this.Configuration.Title}'", LogLevel.Debug);
            }
            this.LogObject.Log($"Program which will be executed: {this.CMD}", LogLevel.Debug);
        }
        private void LogImmediatelyAfterStart(int processId)
        {
            this.LogObject.Log($"Process-Id of started program: " + processId, LogLevel.Debug);
        }
        private void LogException(Exception exception)
        {
            this.LogObject.Log(exception);
        }
        private void LogEnd()
        {
            this.LogObject.Log($"Finished executing program", LogLevel.Debug);
            foreach (string line in Utilities.SplitOnNewLineCharacter(this.GetSummaryOfExecutedProgram()))
            {
                this.LogObject.Log(line, LogLevel.Debug);
            }
        }

        private Process _Process;
        private IDisposable _SubNamespace;
        private Task StartProgram()
        {
            this._SubNamespace = this.LogObject.UseSubNamespace(this.Configuration.LogNamespace);
            this._Process = new Process();
            Stopwatch stopWatch = new();
            try
            {
                this.ProcessWasAbortedDueToTimeout = false;
                if (!Directory.Exists(this.Configuration.WorkingDirectory))
                {
                    throw new ArgumentException($"The specified working-directory '{this.Configuration.WorkingDirectory}' does not exist.");
                }
                ProcessStartInfo StartInfo = new(this.Configuration.Program)
                {
                    UseShellExecute = false,
                    ErrorDialog = false,
                    Arguments = Configuration.Argument,
                    WorkingDirectory = Configuration.WorkingDirectory,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = !this.Configuration.CreateWindow,
                };

                if (Configuration.User != null)
                {

                    System.Security.SecureString password = new System.Security.SecureString();                  
                    StartInfo.UserName = Configuration.User;
                    for (int x = 0; x < Configuration.Password.Length; x++)
                    {
                        password.AppendChar(Configuration.Password[x]);
                    }
                    StartInfo.Password = password;
                }
                if (Configuration.DelegateToEpew)
                {
                    StartInfo.Arguments = ExternalProgramExecutor.CreateEpewArgumentString(
                        Configuration.Program, Configuration.Argument, Configuration.WorkingDirectory, Configuration.PrintErrorsAsInformation, Configuration.TimeoutInMilliseconds, Configuration.Verbosity,
                        Configuration.AddLogOverhead, Configuration.LogFile, Configuration.Title, Configuration.WaitingState, Configuration.LogNamespace, Configuration.User,Configuration.Password);
                    StartInfo.FileName = "Epew";
                }
                this._Process.StartInfo = StartInfo;
                this._Process.OutputDataReceived += (object sender, DataReceivedEventArgs dataReceivedEventArgs) =>
                {
                    this.EnqueueInformation(dataReceivedEventArgs.Data);
                };
                this._Process.ErrorDataReceived += (object sender, DataReceivedEventArgs dataReceivedEventArgs) =>
                {
                    if (this.Configuration.PrintErrorsAsInformation)
                    {
                        this.EnqueueInformation(dataReceivedEventArgs.Data);
                    }
                    else
                    {
                        this.EnqueueError(dataReceivedEventArgs.Data);
                    }
                };
                SupervisedThread readLogItemsThread;
                stopWatch.Start();
                this._Process.Start();
                this.ProcessId = this._Process.Id;
                this.LogImmediatelyAfterStart(this._ProcessId);
                this._Process.BeginOutputReadLine();
                this._Process.BeginErrorReadLine();
                this._Running = true;
                readLogItemsThread = SupervisedThread.Create(this.LogOutputImplementation);
                readLogItemsThread.Name = $"Logger-Thread for '{this.Configuration.Title}' ({nameof(ExternalProgramExecutor)}({this.Configuration.Title}))";
                readLogItemsThread.LogOverhead = false;
                readLogItemsThread.Start();
            }
            catch (Exception exception)
            {
                this.Dispose();
                Exception processStartException = new ProcessStartException($"Exception occurred while start execution '{this.Configuration.Title}'", exception);
                this.LogException(processStartException);
                throw processStartException;
            }
            Task task = new(() =>
            {
                try
                {
                    this.WaitForProcessEnd(this._Process, stopWatch);
                    this.ExecutionDuration = stopWatch.Elapsed;
                    this.ExitCode = this._Process.ExitCode;
                    while (!this._NotLoggedOutputLines.IsEmpty)
                    {
                        Thread.Sleep(60);
                    }
                    this._AllStdOutLinesAsArray = this._AllStdOutLines.ToArray();
                    this._AllStdErrLinesAsArray = this._AllStdErrLines.ToArray();
                    this.LogEnd();
                    try
                    {
                        ExecutionFinishedEvent?.Invoke(this, this.ExitCode);
                    }
                    catch
                    {
                        Utilities.NoOperation();
                    }
                    if (this.Configuration.WaitingState is RunSynchronously runSynchronously && runSynchronously.ThrowErrorIfExitCodeIsNotZero && this.ExitCode != 0)
                    {
                        throw new UnexpectedExitCodeException(this);
                    }
                }
                catch (Exception exception)
                {
                    this.LogObject.Log("Error while finishing program-execution", exception);
                }
                finally
                {
                    this.Dispose();
                }
            });
            task.Start();
            return task;
        }
        public void Dispose()
        {
            if (this._SubNamespace != null)
            {
                this._SubNamespace.Dispose();
            }
            if (this._Process != null)
            {
                this._Process.Dispose();
            }
        }

        private void WaitForProcessEnd(Process process, Stopwatch stopwatch)
        {
            if (this.Configuration.TimeoutInMilliseconds.HasValue)
            {
                if (!process.WaitForExit(this.Configuration.TimeoutInMilliseconds.Value))
                {
                    process.Kill();
                    process.WaitForExit();
                    stopwatch.Stop();
                    this.LogObject.Log($"Execution was aborted due to a timeout. (The timeout was set to {Utilities.DurationToUserFriendlyString(TimeSpan.FromMilliseconds(this.Configuration.TimeoutInMilliseconds.Value))}).", LogLevel.Debug);
                    this.ProcessWasAbortedDueToTimeout = true;
                }
            }
            else
            {
                process.WaitForExit();
                stopwatch.Stop();
            }
            if (process.ExitCode != 0 && this.Configuration.Verbosity == Verbosity.Normal)
            {
                foreach (string stdOutLine in this._AllStdOutLines)
                {
                    this._NotLoggedOutputLines.Enqueue((LogLevel.Information, stdOutLine));
                }
                foreach (string stdErrLine in this._AllStdErrLines)
                {
                    this._NotLoggedOutputLines.Enqueue((LogLevel.Error, stdErrLine));
                }
            }
            this._Running = false;
            this.CurrentExecutionState = ExecutionState.Terminated;
        }
        public void WaitUntilTerminated()
        {
            Utilities.WaitUntilConditionIsTrue(() => this.CurrentExecutionState == ExecutionState.Terminated);
        }
        private void CheckIfStartOperationWasAlreadyCalled()
        {
            lock (this._LockObject)
            {
                if (this.CurrentExecutionState != ExecutionState.NotStarted)
                {
                    throw new InvalidOperationException("The process was already started.");
                }
                this.CurrentExecutionState = ExecutionState.Running;
            }
        }

        private void ResolvePaths()
        {
            Tuple<string, string, string> temp = Utilities.ResolvePathOfProgram(this.Configuration.Program, this.Configuration.Argument, this.Configuration.WorkingDirectory);
            this.Configuration.Program = temp.Item1;
            this.Configuration.Argument = temp.Item2;
            this.Configuration.WorkingDirectory = temp.Item3;
            if (string.IsNullOrWhiteSpace(this.Configuration.WorkingDirectory))
            {
                this.Configuration.WorkingDirectory = Directory.GetCurrentDirectory();
            }
            else
            {
                this.Configuration.WorkingDirectory = Utilities.ResolveToFullPath(this.Configuration.WorkingDirectory);
            }
        }
        private readonly IList<string> _AllStdErrLines = new List<string>();
        private string[] _AllStdErrLinesAsArray;
        /// <exception cref="InvalidOperationException">
        /// If the process is not terminated yet.
        /// </exception>
        public string[] AllStdErrLines
        {
            get
            {
                if (this.CurrentExecutionState == ExecutionState.Terminated)
                {
                    return this._AllStdErrLinesAsArray;
                }
                else
                {
                    throw new InvalidOperationException(this.GetInvalidOperationDueToNotTerminatedMessageByMembername(nameof(this.AllStdErrLines), ExecutionState.Terminated, true));
                }
            }
        }

        private string GetInvalidOperationDueToNotTerminatedMessageByMembername(string name, ExecutionState state, bool requiredIn)
        {
            string requiredInAsString = requiredIn ? "" : " not";
            return $"'{name}' is not avilable because the current {nameof(ExecutionState)}-value state is {Enum.GetName(typeof(ExecutionState), this.CurrentExecutionState)} but it must{requiredInAsString} be in the state {Enum.GetName(typeof(ExecutionState), state)} to be able to query it.";
        }

        private bool _processWasAbortedDueToTimeout;
        /// <exception cref="InvalidOperationException">
        /// If the process is not terminated yet.
        /// </exception>
        public bool ProcessWasAbortedDueToTimeout
        {
            get
            {
                if (this.CurrentExecutionState == ExecutionState.Terminated)
                {
                    return this._processWasAbortedDueToTimeout;
                }
                else
                {
                    throw new InvalidOperationException(this.GetInvalidOperationDueToNotTerminatedMessageByMembername(nameof(this.ProcessWasAbortedDueToTimeout), ExecutionState.Terminated, true));
                }
            }
            private set
            {
                this._processWasAbortedDueToTimeout = value;
            }
        }
        private int _ExitCode;
        /// <exception cref="InvalidOperationException">
        /// If the process is not terminated yet.
        /// </exception>
        public int ExitCode
        {
            get
            {
                if (this.CurrentExecutionState == ExecutionState.Terminated)
                {
                    return this._ExitCode;
                }
                else
                {
                    throw new InvalidOperationException(this.GetInvalidOperationDueToNotTerminatedMessageByMembername(nameof(this.ExitCode), ExecutionState.Terminated, true));
                }
            }
            private set
            {
                this._ExitCode = value;
            }
        }
        private int _ProcessId;
        public int ProcessId
        {
            get
            {
                if (this.CurrentExecutionState != ExecutionState.NotStarted)
                {
                    return this._ProcessId;
                }
                else
                {
                    throw new InvalidOperationException(this.GetInvalidOperationDueToNotTerminatedMessageByMembername(nameof(this.ProcessId), ExecutionState.NotStarted, false));
                }
            }
            private set
            {
                this._ProcessId = value;
            }
        }

        private readonly IList<string> _AllStdOutLines = new List<string>();
        private string[] _AllStdOutLinesAsArray;
        /// <exception cref="InvalidOperationException">
        /// If the process is not terminated yet.
        /// </exception>
        public string[] AllStdOutLines
        {
            get
            {
                if (this.CurrentExecutionState == ExecutionState.Terminated)
                {
                    return this._AllStdOutLinesAsArray;
                }
                else
                {
                    throw new InvalidOperationException(this.GetInvalidOperationDueToNotTerminatedMessageByMembername(nameof(this.AllStdOutLines), ExecutionState.Terminated, true));
                }
            }
        }


        private void EnqueueInformation(string rawLine)
        {
            if (this.NormalizeLine(rawLine, out string line))
            {
                this._AllStdOutLines.Add(line);
                if (this.Configuration.Verbosity == Verbosity.Full || this.Configuration.Verbosity == Verbosity.Verbose)
                {
                    this._NotLoggedOutputLines.Enqueue((LogLevel.Information, line));
                }
            }
        }

        private void EnqueueError(string rawLine)
        {
            if (this.NormalizeLine(rawLine, out string line))
            {
                this._AllStdErrLines.Add(line);
                if (this.Configuration.Verbosity == Verbosity.Full || this.Configuration.Verbosity == Verbosity.Verbose)
                {
                    this._NotLoggedOutputLines.Enqueue((LogLevel.Error, line));
                }
            }
        }

        private bool NormalizeLine(string line, out string data)
        {
            if (line == null)
            {
                data = null;
                return false;
            }
            else
            {
                line = line.Trim();
                if (string.IsNullOrEmpty(line))
                {
                    data = null;
                    return false;
                }
                else
                {
                    data = line;
                    return true;
                }
            }

        }
        private void LogOutputImplementation()
        {
            while (this.IsRunning || !this._NotLoggedOutputLines.IsEmpty)
            {
                if (this._NotLoggedOutputLines.TryDequeue(out (LogLevel, string) logItem))
                {
                    this.LogObject.Log(logItem.Item2, logItem.Item1);
                }
                else
                {
                    Thread.Sleep(50);
                }
            }
        }
    }
    /// <summary>
    /// This enum contains the verbosity-level for <see cref="ExternalProgramExecutor"/>.
    /// </summary>
    public enum Verbosity
    {
        /// <summary>
        /// No output will be logged.
        /// </summary>
        Quiet = 0,
        /// <summary>
        /// If the exitcode of the executed program is not 0 then the StdErr will be logged.
        /// </summary>
        Normal = 1,
        /// <summary>
        /// Logs StdOut and StdErr of the executed program in realtime.
        /// </summary>
        Full = 2,
        /// <summary>
        /// Same as <see cref="Verbosity.Full"/> but with some more information added by <see cref="ExternalProgramExecutor"/>.
        /// </summary>
        Verbose = 3,
    }
    public enum ExecutionState
    {
        NotStarted = 0,
        Running = 1,
        Terminated = 2
    }
}
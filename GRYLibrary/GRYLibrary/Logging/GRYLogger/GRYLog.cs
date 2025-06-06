﻿using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using GRYLibrary.Core.Logging.GRYLogger.ConcreteLogTargets;
using GRYLibrary.Core.Misc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace GRYLibrary.Core.Logging.GRYLogger
{
    public sealed class GRYLog : IGRYLog
    {
        public IGRYLogConfiguration Configuration { get; set; }
        /// <summary>
        /// Represents the basepath for the possibly relative path when accessing <see cref="LogFile.File"/>.
        /// </summary>
        public string BasePath { get; set; }
        private readonly static object _LockObject = new();
        private readonly bool _Initialized = false;
        private int _AmountOfErrors = 0;
        private int _AmountOfWarnings = 0;
        internal readonly ConsoleColor _ConsoleDefaultColor;
        public event NewLogItemEventHandler NewLogItem;
        public delegate void NewLogItemEventHandler(LogItem logItem);
        public event ErrorOccurredEventHandler ErrorOccurred;
        public delegate void ErrorOccurredEventHandler(Exception exception, LogItem logItem);
        private bool AnyLogTargetEnabled
        {
            get
            {
                foreach (GRYLogTarget target in this.Configuration.LogTargets)
                {
                    if (target.Enabled)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        private GRYLog() : this(new GRYLogConfiguration())
        {
        }
        private GRYLog(IGRYLogConfiguration configuration)
        {
            lock (_LockObject)
            {
                this._ConsoleDefaultColor = System.Console.ForegroundColor;
                this.Configuration = configuration;
                this.AddLogEntry = this.Log;
                this._Initialized = true;
            }
        }
        public static GRYLog Create()
        {
            return Create(new GRYLogConfiguration(true));
        }

        public static GRYLog Create(string logFile)
        {
            return Create(GRYLogConfiguration.GetCommonConfiguration(logFile, false));
        }

        public static GRYLog Create(IGRYLogConfiguration configuration)
        {
            return new GRYLog(configuration);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Configuration);
        }

        public override bool Equals(object obj)
        {
            if (obj is not GRYLog typedObject)
            {
                return false;
            }
            if (!this.Configuration.Equals(typedObject.Configuration))
            {
                return false;
            }
            return true;
        }
        /// <remarks>
        /// Will only be used if <see cref="GRYLogConfiguration.StoreProcessedLogItemsInternally"/> is set to true.ls -la
        /// </remarks>
        public IList<LogItem> ProcessedLogItems { get; set; } = new List<LogItem>();
        public Action<LogItem> AddLogEntry { get; set; }

        public int GetAmountOfErrors()
        {
            return this._AmountOfErrors;
        }

        public int GetAmountOfWarnings()
        {
            return this._AmountOfWarnings;
        }

        public void LogGeneralProgramInformation()
        {
            ProcessModule module = Process.GetProcessById(Environment.ProcessId).MainModule;
            this.Log($"Executing assembly-name: {AppDomain.CurrentDomain.FriendlyName} ({module.FileName})", LogLevel.Information);
            this.Log($"Executing file-version: {module.FileVersionInfo.FileVersion}", LogLevel.Information);
            this.Log($"Current working-directory: {Directory.GetCurrentDirectory()}", LogLevel.Information);
        }
        public void LogCurrentSystemStatistics()
        {
            this.Log($"OS description: {RuntimeInformation.OSDescription}", LogLevel.Information);
            string appDrive = Path.GetPathRoot(System.Reflection.Assembly.GetEntryAssembly().Location);
            string cDrive = "C:\\";
            this.LogDriveStatistics(cDrive);
            if (!appDrive.Equals(cDrive))
            {
                this.LogDriveStatistics(appDrive);
            }
        }

        private void LogDriveStatistics(string drive)
        {
            this.Log($"Total free bytes on drive '{drive}': {Utilities.GetTotalFreeSpace(drive)}", LogLevel.Information);//todo print ram usage//todo print cpu usage//todo print if internetconnection exists
        }

        public void LogSummary()
        {
            this.Log($"Amount of occurred Errors and Criticals: {this.GetAmountOfErrors()}", LogLevel.Information);
            this.Log($"Amount of occurred Warnings: {this.GetAmountOfWarnings()}", LogLevel.Information);
        }

        private bool LineShouldBePrinted(string message)
        {
            if (message == null)
            {
                return false;
            }

            if (this.Configuration.PrintEmptyLines)
            {
                return true;
            }
            else
            {
                return !string.IsNullOrWhiteSpace(message);
            }
        }

        public void Log(string message, string messagId = null)
        {
            this.Log(message, LogLevel.Information, messagId);
        }

        public void Log(string message, Exception exception, string messageId = null)
        {
            this.Log(message, LogLevel.Error, exception, messageId);
        }

        public void Log(string message, LogLevel logLevel, Exception exception, string messageId)
        {
            this.Log(new LogItem(message, logLevel, exception, messageId));
        }

        public void Log(string message, LogLevel logLevel, string messageId = null)
        {
            this.Log(() => message, logLevel, messageId);
        }

        public void Log(Func<string> getMessage, string messageId = null)
        {
            this.Log(getMessage, LogLevel.Information, messageId);
        }

        public void Log(Func<string> getMessage, Exception exception, string messageId = null)
        {
            this.Log(getMessage, LogLevel.Error, exception, messageId);
        }

        public void Log(Exception exception, string messageId = null)
        {
            this.Log(LogLevel.Error, exception, messageId);
        }

        public void Log(LogLevel logLevel, Exception exception, string messageId = null)
        {
            this.Log(() => "An exception occurred", logLevel, exception, messageId);
        }

        public void Log(Func<string> getMessage, LogLevel logLevel, Exception exception, string messageId = null)
        {
            this.Log(new LogItem(getMessage(), logLevel, exception, messageId)
            {
                LogTargets = this.Configuration.LogTargets.ToHashSet(),
            });
        }

        public void Log(Func<string> getMessage, LogLevel logLevel, string messageId = null)
        {
            this.Log(new LogItem(getMessage, logLevel, messageId)
            {
                LogTargets = this.Configuration.LogTargets.ToHashSet(),
            });
        }

        public void Log(GRYLogTarget enabledLogTarget, string message, LogLevel logLevel)
        {
            lock (_LockObject)
            {
                Dictionary<string, bool> logTargetsEnabled = this.Configuration.LogTargets.Select(logtarget => (logtarget.GetType().FullName, logtarget.Enabled)).ToDictionary();
                try
                {
                    this.Configuration.LogTargets.ForEach(logtarget => logtarget.Enabled = logtarget.GetType().Equals(enabledLogTarget.GetType()));
                    this.Log(message, logLevel);
                }
                finally
                {
                    this.Configuration.LogTargets.ForEach(logtarget => logtarget.Enabled = logTargetsEnabled[logtarget.GetType().FullName]);
                }
            }
        }
        public void Log(LogItem logitem)
        {
            lock (_LockObject)
            {
                if (this.Configuration.WriteLogEntriesAsynchronous)
                {
                    new Task(() => this.LogImplementation(logitem)).Start();
                }
                else
                {
                    this.LogImplementation(logitem);
                }
            }
        }

        public void LogProgramOutput(string message, string[] stdOutLines, string[] stdErrLines, LogLevel logevel)
        {
            lock (_LockObject)
            {
                this.Log(FormatProgramOutput(message, stdOutLines, stdErrLines), logevel);
            }
        }

        public static string FormatProgramOutput(string message, string[] stdOutLines, string[] stdErrLines)
        {
            return $"{message}; StdOut: {Environment.NewLine}{string.Join(Environment.NewLine, stdOutLines)}; StdErr: {Environment.NewLine}{string.Join(Environment.NewLine, stdErrLines)}";
        }

        private void LogImplementation(LogItem logItem)
        {
            try
            {
                lock (_LockObject)
                {
                    if (LogLevel.None == logItem.LogLevel)
                    {
                        return;
                    }
                    if (!this._Initialized)
                    {
                        return;
                    }
                    if (!this.Configuration.Enabled)
                    {
                        return;
                    }
                    if (!this.IsEnabled(logItem.LogLevel))
                    {
                        return;
                    }
                    if (!this.LineShouldBePrinted(logItem.PlainMessage))
                    {
                        return;
                    }
                    if (this.Configuration.StoreProcessedLogItemsInternally)
                    {
                        this.ProcessedLogItems.Add(logItem);
                    }
                    if (logItem.PlainMessage.Contains(Environment.NewLine) && this.Configuration.LogEveryLineOfLogEntryInNewLine)
                    {
                        foreach (string line in logItem.PlainMessage.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
                        {
                            this.Log(new LogItem(line, logItem.LogLevel, logItem.Exception));
                        }
                        return;
                    }
                    if (this.Configuration.PrintErrorsAsInformation && logItem.LogLevel.Equals(LogLevel.Error))
                    {
                        logItem.LogLevel = LogLevel.Information;
                    }
                    if (this.IsErrorLogLevel(logItem.LogLevel))
                    {
                        this._AmountOfErrors += 1;
                    }
                    if (this.IsWarningLogLevel(logItem.LogLevel))
                    {
                        this._AmountOfWarnings += 1;
                    }
                    foreach (GRYLogTarget logTarget in this.Configuration.LogTargets)
                    {
                        if (logTarget.Enabled)
                        {
                            if (logItem.LogTargets.Contains(logTarget))
                            {

                                if (logTarget.LogLevels.Contains(logItem.LogLevel))
                                {
                                    logTarget.Execute(logItem, this);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorOccurred?.Invoke(exception, logItem);
            }
        }

        private bool IsWarningLogLevel(LogLevel logLevel)
        {
            return logLevel.Equals(LogLevel.Warning);
        }

        private bool IsErrorLogLevel(LogLevel logLevel)
        {
            return logLevel.Equals(LogLevel.Error) || logLevel.Equals(LogLevel.Critical);
        }

        [Obsolete($"Use {nameof(GeneralLoggerExtensions)}.{nameof(GeneralLoggerExtensions.LogLoopExecution)} instead.")]
        public void ExecuteAndLogForEach<T>(IEnumerable<T> items, Action<T> itemAction, string nameOfEntireLoopAction, string subNamespaceOfEntireLoopAction, Func<T, string> nameOfSingleItemFunc, Func<T, string> subNamespaceOfSingleItemFunc, bool preventThrowingExceptions = false, LogLevel logLevelForOverhead = LogLevel.Debug)
        {
            GeneralLoggerExtensions.Log(this, nameOfEntireLoopAction, logLevelForOverhead, !preventThrowingExceptions, true, false, true, true, () =>
            {
                List<T> itemsAsList = items.ToList();
                uint amountOfItems = (uint)itemsAsList.Count;
                for (uint currentIndex = 0; currentIndex < itemsAsList.Count; currentIndex++)
                {
                    try
                    {
                        T currentItem = itemsAsList[(int)currentIndex];
                        string nameOfSingleItem = nameOfSingleItemFunc(currentItem);
                        GeneralLoggerExtensions.Log(this, nameOfSingleItemFunc(currentItem), logLevelForOverhead, !preventThrowingExceptions, true, false, true, false, () => itemAction(currentItem));
                    }
                    finally
                    {
                        this.LogProgress(currentIndex + 1, amountOfItems);
                    }
                }
            });
        }

        public void LogProgress(uint enumerator, uint denominator)
        {
            if (enumerator < 0 || denominator < 0 || denominator < enumerator)
            {
                throw new ArgumentException($"Can not log progress for {nameof(enumerator)}={enumerator} and {nameof(denominator)}={denominator}. Both values must be nonnegative and {nameof(denominator)} must be greater than {nameof(enumerator)}.");
            }
            else
            {
                string percentValue = Math.Round(enumerator / (double)denominator * 100, 2).ToString();
                int denominatorLength = (int)Math.Floor(Math.Log10(denominator) + 1);
                string message = $"Processed {enumerator.ToString().PadLeft(denominatorLength, '0')}/{denominator} items ({percentValue}%)";
                this.Log(message);
            }
        }
        [Obsolete($"Use {nameof(GeneralLoggerExtensions)}.{nameof(GeneralLoggerExtensions.Log)} instead")]
        public void ExecuteAndLog(Action action, string nameOfAction, bool preventThrowingExceptions = false, LogLevel logLevelForOverhead = LogLevel.Debug, string subNamespaceForLog = Utilities.EmptyString)
        {
            GeneralLoggerExtensions.Log(this, nameOfAction, logLevelForOverhead, !preventThrowingExceptions, true, false, true, true, action);
        }

        public TResult ExecuteAndLog<TResult>(Func<TResult> action, string nameOfAction, bool preventThrowingExceptions = false, LogLevel logLevelForOverhead = LogLevel.Debug, TResult defaultValue = default, string subNamespaceForLog = Utilities.EmptyString)
        {
            this.Log($"Action '{nameOfAction}' will be started now.", logLevelForOverhead);
            Stopwatch stopWatch = new();
            try
            {
                using (this.UseSubNamespace(subNamespaceForLog))
                {
                    stopWatch.Start();
                    TResult result = action();
                    stopWatch.Stop();
                    return result;
                }
            }
            catch (Exception exception)
            {
                this.Log($"An exception occurred while executing action '{nameOfAction}'.", LogLevel.Error, exception, 0x78200003.ToString());
                if (preventThrowingExceptions)
                {
                    return defaultValue;
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                this.Log($"Action '{nameOfAction}' finished. Duration: {Utilities.DurationToUserFriendlyString(stopWatch.Elapsed)}", logLevelForOverhead);
            }
        }

        public void Dispose()
        {
        }

        public IDisposable UseSubNamespace(string subnamespace)
        {
            return new GRYLogSubNamespaceProvider(this, subnamespace);
        }

        internal void InvokeObserver(LogItem message)
        {
            try
            {
                NewLogItem?.Invoke(message);
            }
            catch
            {
                Utilities.NoOperation();
            }
        }

        private string FormatEvent(EventId eventId)
        {
            return $"EventId: {eventId.Id}, EventName: '{eventId.Name}'";
        }

        #region ILogger-Implementation

        public bool IsEnabled(LogLevel logLevel)
        {
            return this.AnyLogTargetEnabled;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new GRYLogSubNamespaceProvider(this, state.ToString());
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            this.Log(() => $"{this.FormatEvent(eventId)} | {formatter(state, exception)}", logLevel, 0x78200004.ToString());
        }

        #endregion

    }
}
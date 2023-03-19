using GRYLibrary.Core.AdvancedObjectAnalysis;
using GRYLibrary.Core.Log.ConcreteLogTargets;
using GRYLibrary.Core.Miscellaneous;
using GRYLibrary.Core.XMLSerializer;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Console = GRYLibrary.Core.Log.ConcreteLogTargets.Console;

namespace GRYLibrary.Core.Log
{
    public interface IGRYLogConfiguration : IDisposable
    {
        public List<GRYLogTarget> LogTargets { get; set; }
        public bool WriteLogEntriesAsynchronous { get; set; }
        public bool Enabled { get; set; }
        public bool PrintEmptyLines { get; set; }

        /// <summary>
        /// Represents a value which indicates if the output which goes to stderr should be treated as stdout.       
        /// </summary>
        public bool PrintErrorsAsInformation { get; set; }
        public string Name { get; set; }
        public string DateFormat { get; set; }
        public List<SerializableKeyValuePair<LogLevel, LoggedMessageTypeConfiguration>> LoggedMessageTypesConfiguration { get; set; }
        public bool ConvertTimeForLogentriesToUTCFormat { get; set; }
        public bool LogEveryLineOfLogEntryInNewLine { get; set; }
        public bool StoreProcessedLogItemsInternally { get; set; }
    }
    public class GRYLogConfiguration : IGRYLogConfiguration
    {
        public List<GRYLogTarget> LogTargets { get; set; }
        public bool WriteLogEntriesAsynchronous { get; set; }
        public bool Enabled { get; set; }
        public bool PrintEmptyLines { get; set; }
        public bool PrintErrorsAsInformation { get; set; }
        public string Name { get; set; }
        public string DateFormat { get; set; }
        public List<SerializableKeyValuePair<LogLevel, LoggedMessageTypeConfiguration>> LoggedMessageTypesConfiguration { get; set; }
        public bool ConvertTimeForLogentriesToUTCFormat { get; set; }
        public bool LogEveryLineOfLogEntryInNewLine { get; set; }
        public bool StoreProcessedLogItemsInternally { get; set; }
        public GRYLogConfiguration() : this(false)
        {
        }
        public GRYLogConfiguration(bool initializeWithDefaultValues = false)
        {
            if (initializeWithDefaultValues)
            {
                Initliaze();
            }
        }

        private void Initliaze()
        {
            LogTargets = new List<GRYLogTarget> {
                new Console() { Enabled = true, Format = GRYLogLogFormat.OnlyMessage },
                new LogFile() { Enabled = false, Format = GRYLogLogFormat.GRYLogFormat }
            };
            WriteLogEntriesAsynchronous = false;
            Enabled = true;
            PrintEmptyLines = false;
            PrintErrorsAsInformation = false;
            Name = string.Empty;
            DateFormat = "yyyy-MM-dd HH:mm:ss";
            LoggedMessageTypesConfiguration = new List<SerializableKeyValuePair<LogLevel, LoggedMessageTypeConfiguration>>
            {
                new SerializableKeyValuePair<LogLevel, LoggedMessageTypeConfiguration>(LogLevel.Trace, new LoggedMessageTypeConfiguration() { CustomText = nameof(LogLevel.Trace), ConsoleColor = ConsoleColor.Gray }),
                new SerializableKeyValuePair<LogLevel, LoggedMessageTypeConfiguration>(LogLevel.Debug, new LoggedMessageTypeConfiguration() { CustomText = nameof(LogLevel.Debug), ConsoleColor = ConsoleColor.Cyan }),
                new SerializableKeyValuePair<LogLevel, LoggedMessageTypeConfiguration>(LogLevel.Information, new LoggedMessageTypeConfiguration() { CustomText = nameof(LogLevel.Information), ConsoleColor = ConsoleColor.DarkGreen }),
                new SerializableKeyValuePair<LogLevel, LoggedMessageTypeConfiguration>(LogLevel.Warning, new LoggedMessageTypeConfiguration() { CustomText = nameof(LogLevel.Warning), ConsoleColor = ConsoleColor.DarkYellow }),
                new SerializableKeyValuePair<LogLevel, LoggedMessageTypeConfiguration>(LogLevel.Error, new LoggedMessageTypeConfiguration() { CustomText = nameof(LogLevel.Error), ConsoleColor = ConsoleColor.Red }),
                new SerializableKeyValuePair<LogLevel, LoggedMessageTypeConfiguration>(LogLevel.Critical, new LoggedMessageTypeConfiguration() { CustomText = nameof(LogLevel.Critical), ConsoleColor = ConsoleColor.DarkRed })
            };
            ConvertTimeForLogentriesToUTCFormat = false;
            LogEveryLineOfLogEntryInNewLine = false;
            StoreProcessedLogItemsInternally = false;
        }

        public LoggedMessageTypeConfiguration GetLoggedMessageTypesConfigurationByLogLevel(LogLevel logLevel)
        {
            foreach (SerializableKeyValuePair<LogLevel, LoggedMessageTypeConfiguration> obj in this.LoggedMessageTypesConfiguration)
            {
                if (obj.Key == logLevel)
                {
                    return obj.Value;
                }
            }
            throw new KeyNotFoundException();
        }

        public void AddSystemLog()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                this.LogTargets.Add(new WindowsEventLog());
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                this.LogTargets.Add(new Syslog());
            }
        }
        public Target GetLogTarget<Target>() where Target : GRYLogTarget
        {
            foreach (GRYLogTarget gryLogTarget in this.LogTargets)
            {
                if (gryLogTarget is Target target)
                {
                    return target;
                }
            }
            throw new KeyNotFoundException($"No {typeof(Target).Name}-target available");
        }
        public void SetEnabledOfAllLogTargets(bool newEnabledValue)
        {
            foreach (GRYLogTarget item in this.LogTargets)
            {
                item.Enabled = newEnabledValue;
            }
        }
        public void Dispose()
        {
            foreach (GRYLogTarget target in this.LogTargets)
            {
                target.Dispose();
            }
        }
        public static GRYLogConfiguration GetCommonConfiguration(string logFile = null, bool verbose = false)
        {
            GRYLogConfiguration result = new GRYLogConfiguration(true);
            if (logFile != null)
            {
                var filelog = result.GetLogTarget<LogFile>();
                filelog.File = logFile;
                filelog.Enabled = true;
            }
            if (verbose)
            {
                foreach (var logLevel in result.LogTargets)
                {
                    logLevel.LogLevels.Add(LogLevel.Debug);
                }
            }
            return result;
        }
        #region Overhead
        public override bool Equals(object @object)
        {
            return Generic.GenericEquals(this, @object);
        }

        public override int GetHashCode()
        {
            return Generic.GenericGetHashCode(this);
        }

        public override string ToString()
        {
            return Generic.GenericToString(this);
        }
        public ISet<Type> GetExtraTypesWhichAreRequiredForSerialization()
        {
            return new HashSet<Type>();
        }
        #endregion
    }

}

﻿using GRYLibrary.Core.AdvancedObjectAnalysis;
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
    public sealed class GRYLogConfiguration : IDisposable
    {



        /// <summary>
        /// If this value is false then changing this value in the configuration-file has no effect.
        /// </summary>
        public bool ReloadConfigurationWhenConfigurationFileWillBeChanged { get; set; }
        public List<GRYLogTarget> LogTargets { get; set; }
        public bool WriteLogEntriesAsynchronous { get; set; }
        public bool Enabled { get; set; }
        public string ConfigurationFile { get; set; }
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

        public GRYLogConfiguration() { }
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

        public void SetLogFile(string file)
        {
            this.GetLogTarget<LogFile>().File = file;
        }
        public string GetLogFile()
        {
            return this.GetLogTarget<LogFile>().File;
        }
        public void SetFormat(GRYLogLogFormat format)
        {
            foreach (GRYLogTarget target in this.LogTargets)
            {
                target.Format = format;
            }
        }

        public void ResetToDefaultValues()
        {
            this.ResetToDefaultValues(null);
        }
        public void ResetToDefaultValues(string logFile)
        {
            this.ReloadConfigurationWhenConfigurationFileWillBeChanged = true;
            this.LogTargets = new List<GRYLogTarget>();
            this.WriteLogEntriesAsynchronous = false;
            this.Enabled = true;
            this.ConfigurationFile = string.Empty;
            this.PrintEmptyLines = false;
            this.PrintErrorsAsInformation = false;
            this.DateFormat = "yyyy-MM-dd HH:mm:ss";
            this.ConvertTimeForLogentriesToUTCFormat = false;
            this.LogEveryLineOfLogEntryInNewLine = false;
            this.Name = string.Empty;
            this.StoreProcessedLogItemsInternally = false;
            this.LoggedMessageTypesConfiguration = new List<SerializableKeyValuePair<LogLevel, LoggedMessageTypeConfiguration>>();
            this.LoggedMessageTypesConfiguration = new List<SerializableKeyValuePair<LogLevel, LoggedMessageTypeConfiguration>>
            {
                new SerializableKeyValuePair<LogLevel, LoggedMessageTypeConfiguration>(LogLevel.Trace, new LoggedMessageTypeConfiguration() { CustomText = nameof(LogLevel.Trace), ConsoleColor =  ConsoleColor.Gray }),
                new SerializableKeyValuePair<LogLevel, LoggedMessageTypeConfiguration>(LogLevel.Debug, new LoggedMessageTypeConfiguration() { CustomText = nameof(LogLevel.Debug), ConsoleColor = ConsoleColor.Cyan }),
                new SerializableKeyValuePair<LogLevel, LoggedMessageTypeConfiguration>(LogLevel.Information, new LoggedMessageTypeConfiguration() { CustomText = nameof(LogLevel.Information), ConsoleColor = ConsoleColor.DarkGreen }),
                new SerializableKeyValuePair<LogLevel, LoggedMessageTypeConfiguration>(LogLevel.Warning, new LoggedMessageTypeConfiguration() { CustomText = nameof(LogLevel.Warning), ConsoleColor = ConsoleColor.DarkYellow }),
                new SerializableKeyValuePair<LogLevel, LoggedMessageTypeConfiguration>(LogLevel.Error, new LoggedMessageTypeConfiguration() { CustomText = nameof(LogLevel.Error), ConsoleColor = ConsoleColor.Red }),
                new SerializableKeyValuePair<LogLevel, LoggedMessageTypeConfiguration>(LogLevel.Critical, new LoggedMessageTypeConfiguration() { CustomText = nameof(LogLevel.Critical), ConsoleColor = ConsoleColor.DarkRed })
            };
            if (!string.IsNullOrWhiteSpace(logFile))
            {
                string logFolder = Path.GetDirectoryName(logFile);
                if (!string.IsNullOrWhiteSpace(logFolder))
                {
                    Utilities.EnsureDirectoryExists(logFolder);
                }
            }
            this.LogTargets = new List<GRYLogTarget>
            {
                new Console() { Enabled = true, Format = GRYLogLogFormat.OnlyMessage },
                new LogFile() { Enabled = !string.IsNullOrWhiteSpace(logFile), Format = GRYLogLogFormat.GRYLogFormat, File = logFile }
            };
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
        public static void SaveConfiguration(string configurationFile, GRYLogConfiguration configuration)
        {
            Utilities.PersistToDisk(configuration, configurationFile).SaveObjectToFile();
        }
        public static GRYLogConfiguration LoadConfiguration(string configurationFile)
        {
            return Utilities.LoadFromDisk<GRYLogConfiguration>(configurationFile).Object;
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

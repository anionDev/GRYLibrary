using GRYLibrary.Core.AdvancedObjectAnalysis;
using GRYLibrary.Core.Log.ConcreteLogTargets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace GRYLibrary.Core.Log
{
    [XmlInclude(typeof(ConcreteLogTargets.Console))]
    [XmlInclude(typeof(LogFile))]
    [XmlInclude(typeof(Observer))]
    [XmlInclude(typeof(WindowsEventLog))]
    public abstract class GRYLogTarget :IDisposable
    {
        public GRYLogLogFormat Format { get; set; } = GRYLogLogFormat.GRYLogFormat;

        public HashSet<LogLevel> LogLevels { get; set; } = new HashSet<LogLevel>
            {
                 LogLevel.Information,
                 LogLevel.Warning,
                 LogLevel.Error,
                 LogLevel.Critical
            };
        public bool Enabled { get; set; } = true;
        public abstract HashSet<Type> FurtherGetExtraTypesWhichAreRequiredForSerialization();
        internal void Execute(LogItem logItem, GRYLog logObject)
        {
            this.ExecuteImplementation(logItem, logObject);
        }
        protected abstract void ExecuteImplementation(LogItem logItem, GRYLog logObject);
        #region Overhead
        public override bool Equals(object @object)
        {
            return @object is not null && @object.GetType().Equals(this.GetType());
        }

        public override int GetHashCode()
        {
            return this.GetType().GetHashCode();
        }

        public override string ToString()
        {
            return Generic.GenericToString(this);
        }

        public abstract void Dispose();

        internal static ISet<GRYLogTarget> GetAll()
        {
            HashSet<GRYLogTarget> result = new HashSet<GRYLogTarget>();
            result.Add(new ConcreteLogTargets. Console());
            result.Add(new LogFile());
            result.Add(new Observer());
            result.Add(new Syslog());
            result.Add(new WindowsEventLog());
            return result;
        }
        #endregion
    }
}
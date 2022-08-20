﻿using GRYLibrary.Core.AdvancedObjectAnalysis;
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
    public abstract class GRYLogTarget : IDisposable
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

        public abstract void Dispose();
        #endregion
    }
}
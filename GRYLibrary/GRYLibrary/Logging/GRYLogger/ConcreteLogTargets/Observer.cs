﻿using GRYLibrary.Core.Misc;
using System;
using System.Collections.Generic;

namespace GRYLibrary.Core.Logging.GRYLogger.ConcreteLogTargets
{
    public sealed class Observer : GRYLogTarget
    {
        public Observer() { }

        public override HashSet<Type> FurtherGetExtraTypesWhichAreRequiredForSerialization() => new HashSet<Type>();

        protected override void ExecuteImplementation(LogItem logItem, GRYLog logObject) => logObject.InvokeObserver(logItem);
        public override void Dispose() => Utilities.NoOperation();
        public override bool Equals(object obj) => base.Equals(obj);

        public override int GetHashCode() => base.GetHashCode();
    }
}
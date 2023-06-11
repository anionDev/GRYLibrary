﻿using GRYLibrary.Core.Miscellaneous;
using System;
using System.Collections.Generic;

namespace GRYLibrary.Core.Log.ConcreteLogTargets
{
    public sealed class Observer :GRYLogTarget
    {
        public Observer() { }

        public override HashSet<Type> FurtherGetExtraTypesWhichAreRequiredForSerialization()
        {
            return new HashSet<Type>();
        }

        protected override void ExecuteImplementation(LogItem logItem, GRYLog logObject)
        {
            logObject.InvokeObserver(logItem);
        }
        public override void Dispose()
        {
            Utilities.NoOperation();
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
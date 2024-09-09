using GRYLibrary.Core.AOA;
using System;

namespace GRYLibrary.Core.Logging.GRYLogger
{
    public class LoggedMessageTypeConfiguration
    {
        public ConsoleColor ConsoleColor { get; set; }
        public string CustomText { get; set; }
        public LoggedMessageTypeConfiguration() { }
        #region Overhead
        public override bool Equals(object @object) => Generic.GenericEquals(this, @object);

        public override int GetHashCode() => Generic.GenericGetHashCode(this);

        public override string ToString() => Generic.GenericToString(this);
        #endregion
    }
}
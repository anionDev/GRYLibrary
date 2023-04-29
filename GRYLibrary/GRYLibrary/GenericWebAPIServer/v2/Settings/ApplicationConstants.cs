using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;
using GRYLibrary.Core.GenericWebAPIServer.ExecutionModes;
using GRYLibrary.Core.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.v2.Settings
{
    /// <summary>
    /// Represents Application-constants which are not editable by a configuration-file.
    /// </summary>
    public interface IApplicationConstants<AppSpecificConstants>
    {
        public string ApplicationName { get; set; }
        public Version3 ApplicationVersion { get; set; }
        public ExecutionMode ExecutionMode { get; set; }
        public GRYEnvironment Environment { get; set; }
        public AppSpecificConstants ApplicationSpecificConstants { get; set; }
    }
    public class ApplicationConstants<AppSpecificConstants> :IApplicationConstants<AppSpecificConstants>
    {
        public string ApplicationName { get; set; }
        public Version3 ApplicationVersion { get; set; }
        public ExecutionMode ExecutionMode { get; set; }
        public GRYEnvironment Environment { get; set; }
        public AppSpecificConstants ApplicationSpecificConstants { get; set; }
    }
}

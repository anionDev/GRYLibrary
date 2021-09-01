using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer
{
    public interface IAdministrationSettings
    {
        public string ProgramName { get; set; }
        public Version Version { get; set; }
        public IEnvironment Environment{ get; set; }
    }
}

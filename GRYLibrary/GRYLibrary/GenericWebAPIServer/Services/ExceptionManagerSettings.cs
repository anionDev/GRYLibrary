using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.Services
{
    public class ExceptionManagerSettings: IExceptionManagerSettings
    {
        public bool Enabled { get; set; } = false;
    }
}

using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;
using GRYLibrary.Core.GenericWebAPIServer.Middlewares;
using GRYLibrary.Core.XMLSerializer;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    public class WebAPIConfigurationValues
    {
        public WebAPIConfigurationConstants WebAPIConfigurationConstants { get; set; }
        public WebAPIConfigurationVariables WebAPIConfigurationVariables { get; set; }
    }
}

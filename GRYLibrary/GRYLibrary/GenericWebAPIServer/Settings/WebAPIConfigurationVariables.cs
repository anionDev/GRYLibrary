using GRYLibrary.Core.XMLSerializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    public class WebAPIConfigurationVariables
    {
        public ApplicationSettings ApplicationSettings { get; set; } = new ApplicationSettings();
        public WebServerSettings WebServerSettings { get; set; } = new WebServerSettings();
     }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.CommonRoutes
{
    public interface  ICommonRoutesInformation
    {
        public string TermsOfServiceLink { get; set; }
        public string ContactLink { get; set; }
        public string LicenseLink { get; set; }
    }
}

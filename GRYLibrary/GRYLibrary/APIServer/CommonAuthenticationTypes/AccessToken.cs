using System;

namespace GRYLibrary.Core.APIServer.CommonAuthenticationTypes
{
    public class AccessToken
    {
        public string Value { get; set; }
        public DateTime ExpiredMoment { get; set; }
    }
}

using GRYLibrary.Core.APIServer.CommonDBTypes;
using System.Collections.Generic;

namespace GRYLibrary.Core.APIServer.CommonAuthenticationTypes
{
    public class UserBackendInformation
    {
        public User User { get; set; }
        public string Password { get; set; }
        public bool UserIsActivated { get; set; } = true;
        public IDictionary<string, RefreshToken> RefreshToken { get; set; } = new Dictionary<string, RefreshToken>();
        public IDictionary<string, AccessToken> AccessToken { get; set; } = new Dictionary<string, AccessToken>();
    }
}

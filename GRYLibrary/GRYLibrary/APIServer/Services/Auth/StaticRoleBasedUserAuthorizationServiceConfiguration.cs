using System.Collections.Generic;

namespace GRYLibrary.Core.APIServer.Services.Auth
{
    public class StaticRoleBasedUserAuthorizationServiceConfiguration : IStaticRoleBasedUserAuthorizationServiceConfiguration
    {
        public Dictionary<string, ISet<string>> AuthorizedGroups { get ; set ; }
    }
}

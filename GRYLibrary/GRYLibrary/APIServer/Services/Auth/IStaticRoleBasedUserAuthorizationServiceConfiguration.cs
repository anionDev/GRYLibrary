using System.Collections.Generic;

namespace GRYLibrary.Core.APIServer.Services.Auth
{
    public interface IStaticRoleBasedUserAuthorizationServiceConfiguration
    {
        /// <summary>
        /// Key: Action
        /// Value: Authorized groups
        /// </summary>
        public Dictionary<string, ISet<string>> AuthorizedGroups { get; set; }
    }
}

using System.Collections.Generic;

namespace GRYLibrary.Core.APIServer.MidT.Auth
{
    public interface IAuthenticationConfiguration : IMiddlewareConfiguration
    {
        public ISet<string> RoutesWhereUnauthenticatedAccessIsAllowed { get; set; } 
    }
}

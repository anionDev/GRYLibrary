using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Keycloak.Net;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.Systems.KeyCloak
{
    public interface IKeyCloakService
    {
        public IKeyCloakServiceSettings Settings { get; }
    }
}

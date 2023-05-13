﻿using Keycloak.Net;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.Systems.KeyCloak
{
    public interface IKeyCloakService
    {
        public IKeyCloakServiceSettings Settings { get; }
        public KeycloakClient GetKeycloakClient();
    }
}

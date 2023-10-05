﻿using Keycloak.Net;

namespace GRYLibrary.Core.APIServer.Services.KeyCloak
{
    public interface IKeyCloakService
    {
        public IKeyCloakServiceSettings Settings { get; }
        public KeycloakClient GetKeycloakClient();
    }
}
﻿namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.Systems.KeyCloak
{
    public interface IKeyCloakServiceSettings
    {
        public string URL { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }
}

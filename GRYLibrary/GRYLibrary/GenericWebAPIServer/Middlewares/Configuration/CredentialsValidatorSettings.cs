using System;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.Configuration
{
    public class CredentialsValidatorSettings :ICredentialsValidatorSettings
    {
        public bool Enabled { get; set; } = true;
    }
}
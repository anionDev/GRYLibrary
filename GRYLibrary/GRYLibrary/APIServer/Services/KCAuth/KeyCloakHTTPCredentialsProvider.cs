using GRYLibrary.Core.APIServer.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System;

namespace GRYLibrary.Core.APIServer.Services.KCAuth
{
    public class KeyCloakHTTPCredentialsProvider : IHTTPCredentialsProvider
    {
        public bool ContainsCredentials(HttpContext context)
        {
            throw new NotImplementedException();
        }

        public string ExtractSecret(HttpContext context)
        {
            throw new NotImplementedException();
        }
    }
}

using GRYLibrary.Core.APIServer.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System;

namespace GRYLibrary.Core.APIServer.Services.OpenIDConnectAuth
{
    public class OpenIDConnectHTTPCredentialsProvider : IHTTPCredentialsProvider
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

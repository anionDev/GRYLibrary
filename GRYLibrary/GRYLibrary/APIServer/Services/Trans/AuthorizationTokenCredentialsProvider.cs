using GRYLibrary.Core.APIServer.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.Services.Trans
{
    public class AuthorizationTokenCredentialsProvider : IHTTPCredentialsProvider
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

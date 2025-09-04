using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.Services.Aut.Prov.H
{
    public class HeaderProvider : CredentialsProviderBase, IHeaderProvider
    {
        public override bool ContainsCredentials(HttpContext context)
        {
            throw new NotImplementedException();
        }

        public override string ExtractSecret(HttpContext context)
        {
            throw new NotImplementedException();
        }

        public bool IsApplicable(HttpContext context)
        {
            throw new NotImplementedException();
        }
    }
}

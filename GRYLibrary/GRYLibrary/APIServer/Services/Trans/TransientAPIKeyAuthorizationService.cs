﻿using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Utilities;
using Microsoft.AspNetCore.Http;
using System;

namespace GRYLibrary.Core.APIServer.Services.Trans
{
    /// <summary>
    /// This is a transient <see cref="IAPIKeyAuthorizationService"/> for testing purposes.
    /// </summary>
    /// <remarks>
    /// Do not use this service in productive-mode because this service does not implement any features to increase security.
    /// </remarks>
    public class TransientAPIKeyAuthorizationService : IAPIKeyAuthorizationService
    {
        public bool IsAuthorized(string action, string secret)
        {
            throw new NotImplementedException();
        }

        public bool IsAuthorized(HttpContext context, AuthorizeAttribute authorizedAttribute)
        {
            throw new NotImplementedException();
        }
    }
}

﻿using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace GRYLibrary.Core.APIServer.MidT.Auth
{
    public class AuthenticationConfiguration : IAuthenticationConfiguration
    {
        public bool Enabled { get; set; } = true;
        public ISet<string> RoutesWhereUnauthenticatedAccessIsAllowed { get; set; } = new HashSet<string>();

        public virtual ISet<FilterDescriptor> GetFilter()
        {
            return new HashSet<FilterDescriptor>();
        }
    }
}
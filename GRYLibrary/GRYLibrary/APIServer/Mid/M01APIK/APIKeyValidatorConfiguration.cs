﻿using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;

namespace GRYLibrary.Core.APIServer.Mid.NewFolder
{
    public abstract class APIKeyValidatorConfiguration : IAPIKeyValidatorConfiguration
    {
        public bool Enabled { get; set; } = true;

        public ISet<FilterDescriptor> GetFilter() => new HashSet<FilterDescriptor>() { new FilterDescriptor { Type = typeof(APIKeyValidatorFilter), Arguments = Array.Empty<object>() } };
    }
}
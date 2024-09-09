﻿using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace GRYLibrary.Core.APIServer.MidT.Obfuscation
{
    public class ObfuscationConfiguration : IObfuscationConfiguration
    {
        public bool Enabled { get; set; } = true;
        public ISet<FilterDescriptor> GetFilter() => new HashSet<FilterDescriptor>();
    }
}
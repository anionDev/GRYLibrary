﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace GRYLibrary.Core.APIServer.Utilities
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute
    {
        public ISet<string> Groups { get; private set; }
        public AuthorizeAttribute() : this(null)
        {
        }
        public AuthorizeAttribute(string groups)
        {
            if (groups == null || string.IsNullOrEmpty(groups))
            {
                this.Groups = new HashSet<string>();
            }
            else
            {
                this.Groups = groups.Split(",").ToHashSet();
            }
        }
    }
}

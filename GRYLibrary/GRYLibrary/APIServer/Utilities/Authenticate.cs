using System;
using System.Collections.Generic;
using System.Linq;

namespace GRYLibrary.Core.APIServer.Utilities
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AuthenticateAttribute : Attribute
    {
        public ISet<string> Groups { get; private set; }
        public AuthenticateAttribute() : this(null)
        {
        }
        public AuthenticateAttribute(params string[] groups)
        {
            if (groups == null)
            {
                this.Groups = new HashSet<string>();
            }
            else
            {
                this.Groups = groups.ToHashSet();
            }
        }
    }
}

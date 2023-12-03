using System;
using System.Collections.Generic;
using System.Linq;

namespace GRYLibrary.Core.APIServer.Mid.Aut
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute
    {
        public ISet<string> Groups { get; private set; }
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.Mid.Aut
{
    public class AuthorizeAttribute : Attribute
    {
        public string Groups { get; private set; }
        public AuthorizeAttribute(string groups)
        {
            this.Groups = groups;
        }
    }
}

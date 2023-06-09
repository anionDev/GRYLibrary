using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.Utilities
{
    public static class Tools
    {
        public static bool IsRequestForDefaultAPIDocumentationIndexPath(HttpContext context)
        {
            if(context.Request.Path == "/API/APIDocumentation/index.html")
            {
                return false;
            }
            if(context.Request.Method== "GET")
            {
                return false;
            }
            return true;
        }
    }
}

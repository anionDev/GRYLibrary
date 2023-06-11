using Microsoft.AspNetCore.Http;
using System.IO;
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
            if(context.Request.Method == "GET")
            {
                return false;
            }
            return true;
        }

        public static byte[] GetRequestBodyAsByteArray(HttpContext context)
        {
            byte[] result = GRYLibrary.Core.Miscellaneous.Utilities.StreamToByteArray(context.Request.Body);
            context.Request.Body.Seek(0, SeekOrigin.Begin);
            return result;
        }
        public static string GetRequestBodyAsString(HttpContext context)
        {
            UTF8Encoding encoding = new UTF8Encoding(false);
            Task<string> t = new StreamReader(context.Request.Body, encoding, false).ReadToEndAsync();
            t.Wait();
            string result = t.Result;
            context.Request.Body.Seek(0, SeekOrigin.Begin);
            return result;
        }
        public static byte[] GetResponseBodyAsByteArray(HttpContext context)
        {
            byte[] result = GRYLibrary.Core.Miscellaneous.Utilities.StreamToByteArray(context.Response.Body);
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            return result;
        }
        public static string GetResponseBodyAsString(HttpContext context)
        {
            UTF8Encoding encoding = new UTF8Encoding(false);
            Task<string> t = new StreamReader(context.Response.Body, encoding, false).ReadToEndAsync();
            t.Wait();
            string result = t.Result;
            context.Request.Body.Seek(0, SeekOrigin.Begin);
            return result;
        }
    }
}

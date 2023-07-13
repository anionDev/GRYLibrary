using GRYLibrary.Core.GenericWebAPIServer.Settings.Configuration;
using GRYLibrary.Core.Miscellaneous;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.Utilities
{
    public static class Tools
    {
        public static async Task<byte[]> GetRequestBodyAsByteArray(HttpContext context)
        {
            string result = await GetRequestBodyAsString(context);
            UTF8Encoding encoding = new UTF8Encoding(false);
            return encoding.GetBytes(result);
        }
        public static async Task<string> GetRequestBodyAsString(HttpContext context)
        {
            context.Request.EnableBuffering();
            UTF8Encoding encoding = new UTF8Encoding(false);
            string result = await new StreamReader(context.Request.Body, encoding, false).ReadToEndAsync();
            context.Request.Body = new MemoryStream(encoding.GetBytes(result));
            return result;
        }
        public static async Task<byte[]> GetResponseBodyAsByteArray(HttpContext context)
        {
            UTF8Encoding encoding = new UTF8Encoding(false);
            string resultString = await GetResponseBodyAsString(context);
            var result = encoding.GetBytes(resultString);
            return result;
        }
        public static Task<string> GetResponseBodyAsString(HttpContext context)
        {
            UTF8Encoding encoding = new UTF8Encoding(false);
            string responseBody;
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            using(var reader = new StreamReader(context.Response.Body))
            {
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                responseBody = reader.ReadToEnd();
            }
            context.Response.Body = new MemoryStream(encoding.GetBytes(responseBody));

            return Task.FromResult(responseBody);

        }
    }
}

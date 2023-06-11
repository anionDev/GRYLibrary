using Microsoft.AspNetCore.Http;
using System;
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
            string result = await GetResponseBodyAsString(context);
            UTF8Encoding encoding = new UTF8Encoding(false);
            return encoding.GetBytes(result);
        }
        public static async Task<string> GetResponseBodyAsString(HttpContext context)
        {
            UTF8Encoding encoding = new UTF8Encoding(false);
            Stream originalBodyStream = context.Response.Body;
            using MemoryStream responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            string responseBody = await new StreamReader(context.Response.Body, encoding, false).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            await responseBodyStream.CopyToAsync(originalBodyStream);
            return responseBody;
        }
        /*
        public static async Task Invoke(HttpContext context)
        {
            DateTime moment = DateTime.Now;
            UTF8Encoding encoding = new UTF8Encoding(false);
            context.Request.EnableBuffering();
            string requestBody = await new StreamReader(context.Request.Body, encoding, false).ReadToEndAsync();
            context.Request.Body.Seek(0, SeekOrigin.Begin);
            Stream originalBodyStream = context.Response.Body;
            using MemoryStream responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;
            await this._Next(context);
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            string responseBody = await new StreamReader(context.Response.Body, encoding, false).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            await responseBodyStream.CopyToAsync(originalBodyStream);
        }
        */
    }
}

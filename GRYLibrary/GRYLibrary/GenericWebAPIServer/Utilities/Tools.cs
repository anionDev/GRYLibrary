using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;
using GRYLibrary.Core.GenericWebAPIServer.ExecutionModes;
using GRYLibrary.Core.GenericWebAPIServer.Settings;
using GRYLibrary.Core.GenericWebAPIServer.Settings.Configuration;
using GRYLibrary.Core.Miscellaneous;
using GRYLibrary.Core.Miscellaneous.ConsoleApplication;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.Utilities
{
    public static class Tools
    {
        public static (byte[] requestBody, byte[] responsetBody) ExecuteAndGetBody(RequestDelegate next, HttpContext context, Func<byte[], byte[]> responseBodyUpdater = null)
        {
            byte[] requestBody = GetRequestBody(context);
            byte[] responseBody;
            Stream originalResponseBody = context.Response.Body;
            using(MemoryStream intermediateResponseBody = new MemoryStream())
            {
                context.Response.Body = intermediateResponseBody;

                Task result = next(context);
                result.Wait();

                //read response body
                intermediateResponseBody.Position = 0;
                responseBody = Miscellaneous.Utilities.StreamToByteArray(intermediateResponseBody);
                if(responseBodyUpdater != null)
                {
                    responseBody = responseBodyUpdater(responseBody);
                }

                //write response body to original response-stream
                intermediateResponseBody.Position = 0;
                using MemoryStream copyStream = new MemoryStream(responseBody);
                copyStream.CopyToAsync(originalResponseBody).Wait();
            }
            context.Response.Body = originalResponseBody;
            return (requestBody, responseBody);
        }
        public static byte[] GetRequestBody(HttpContext context)
        {
            context.Request.EnableBuffering();
            byte[] result = Miscellaneous.Utilities.StreamToByteArray(context.Request.Body);
            context.Request.Body = new MemoryStream(result);
            return result;
        }
        public static bool IsAPIDocumentationRequest(HttpContext context)
        {
            return context.Request.Path.ToString().StartsWith($"{ServerConfiguration.APIRoutePrefix}/{ServerConfiguration.ResourcesSubPath}/{ServerConfiguration.APISpecificationDocumentName}/");
        }
        public static int Create<GCodeUnitSpecificCommandlineParameter, GCodeUnitSpecificConstants, GCodeUnitSpecificConfiguration>(string codeUnitName, string codeUnitDescription, Version3 codeUnitVersion, string[] commandlineArguments, GRYEnvironment environmentTargetType, Func<GCodeUnitSpecificCommandlineParameter, APIServerInitializer<GCodeUnitSpecificConstants, GCodeUnitSpecificConfiguration, GCodeUnitSpecificCommandlineParameter>> initializer)
            where GCodeUnitSpecificConfiguration : new()
            where GCodeUnitSpecificConstants : new()
            where GCodeUnitSpecificCommandlineParameter : class, ICommandlineParameter, new()
        {
            ExecutionMode executionMode = Miscellaneous.Utilities.GetExecutionMode();
            return new GRYConsoleApplication<GCodeUnitSpecificCommandlineParameter, APIServerInitializer<GCodeUnitSpecificConstants, GCodeUnitSpecificConfiguration, GCodeUnitSpecificCommandlineParameter>>(WebAPIServer<GCodeUnitSpecificConstants, GCodeUnitSpecificConfiguration, GCodeUnitSpecificCommandlineParameter>.WebAPIMain, codeUnitName, codeUnitVersion.ToString(), codeUnitDescription, true, executionMode).Main(commandlineArguments, initializer);
        }
    }
}

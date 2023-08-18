using GRYLibrary.Core.APIServer.ConcreteEnvironments;
using GRYLibrary.Core.APIServer.ExecutionModes;
using GRYLibrary.Core.APIServer.Settings;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.Miscellaneous;
using GRYLibrary.Core.Miscellaneous.ConsoleApplication;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.Utilities
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
        public static int Create<GCodeUnitSpecificCommandlineParameter, GCodeUnitSpecificConstants, GCodeUnitSpecificConfiguration>(string codeUnitName, string codeUnitDescription, Version3 codeUnitVersion, GRYEnvironment environmentTargetType, ExecutionMode executionMode, string[] commandlineArguments, Action<ConfigurationInformation<GCodeUnitSpecificConstants, GCodeUnitSpecificConfiguration, GCodeUnitSpecificCommandlineParameter>> initializer)
            where GCodeUnitSpecificConfiguration : new()
            where GCodeUnitSpecificConstants : new()
            where GCodeUnitSpecificCommandlineParameter : class, ICommandlineParameter, new()
        {
            GRYConsoleApplication<GCodeUnitSpecificCommandlineParameter, ConfigurationInformation<GCodeUnitSpecificConstants, GCodeUnitSpecificConfiguration, GCodeUnitSpecificCommandlineParameter>> consoleApp = new GRYConsoleApplication<GCodeUnitSpecificCommandlineParameter, ConfigurationInformation<GCodeUnitSpecificConstants, GCodeUnitSpecificConfiguration, GCodeUnitSpecificCommandlineParameter>>(WebAPIServer<GCodeUnitSpecificConstants, GCodeUnitSpecificConfiguration, GCodeUnitSpecificCommandlineParameter>.WebAPIMain, codeUnitName, codeUnitVersion.ToString(), codeUnitDescription, true, executionMode, environmentTargetType);
            return consoleApp.Main(commandlineArguments, initializer);
        }
    }
}

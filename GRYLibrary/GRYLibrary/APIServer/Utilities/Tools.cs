using GRYLibrary.Core.APIServer.ConcreteEnvironments;
using GRYLibrary.Core.APIServer.ExecutionModes;
using GRYLibrary.Core.APIServer.Settings;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using GRYLibrary.Core.Miscellaneous;
using GRYLibrary.Core.Miscellaneous.ConsoleApplication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using GUtilities = GRYLibrary.Core.Miscellaneous.Utilities;
using System;
using System.IO;
using System.Threading;
using System.Text;

namespace GRYLibrary.Core.APIServer.Utilities
{
    public static class Tools
    {
        public static (byte[] requestBody, byte[] responseBody) ExecuteNextMiddlewareAndGetRequestAndResponseBody(HttpContext context, RequestDelegate next, Func<byte[], byte[]> requestBodyUpdater = null, Func<byte[], byte[]> responseBodyUpdater = null)
        {
            var requestBody = GetRequestBody(context, requestBodyUpdater);
            Stream originalBody = context.Response.Body;
            byte[] responseBody;
            using (var memStream = new MemoryStream())
            {
                context.Response.Body = memStream;
                next(context).Wait();
                memStream.Position = 0;
                responseBody = GUtilities.StreamToByteArray(memStream);
                string temp1= new UTF8Encoding(false).GetString(responseBody);
                context.Response.Body = new MemoryStream(responseBody);
            }

            /*

            byte[] responseBody = default;


                string temp1 = new UTF8Encoding(false).GetString(responseBody);
                if (responseBodyUpdater != null)
                {
                    responseBody = responseBodyUpdater(responseBody);
                }
                string temp2 = new UTF8Encoding(false).GetString(responseBody);
                MemoryStream memStream2 = new MemoryStream(responseBody);
                memStream2.CopyToAsync(originalBody).Wait();

            context.Response.Body = originalBody;
            */
            return (requestBody, responseBody);
        }

        private static (byte[] requestBody, byte[] responseBody) ExecuteNextMiddlewareAndGetRequestAndResponseBody2(HttpContext context, RequestDelegate next, Func<byte[], byte[]> requestBodyUpdater = null, Func<byte[], byte[]> responseBodyUpdater = null)
        {
            var requestBody = GetRequestBody(context, requestBodyUpdater);
            byte[] responseBody = default;
            Stream originalBody = context.Response.Body;

            using (var memStream = new MemoryStream())
            {
                context.Response.Body = memStream;

                next(context).Wait();

                memStream.Position = 0;
                responseBody = GUtilities.StreamToByteArray(memStream);
                string temp1 = new UTF8Encoding(false).GetString(responseBody);
                if (responseBodyUpdater != null)
                {
                    responseBody = responseBodyUpdater(responseBody);
                }
                string temp2 = new UTF8Encoding(false).GetString(responseBody);
                MemoryStream memStream2 = new MemoryStream(responseBody);
                memStream2.CopyToAsync(originalBody).Wait();
            }

            context.Response.Body = originalBody;
            return (requestBody, responseBody);
        }
        public static byte[] GetRequestBody(HttpContext context, Func<byte[], byte[]> requestBodyUpdater = null)
        {
            context.Request.EnableBuffering();
            byte[] result = GUtilities.StreamToByteArray(context.Request.Body);
            if (requestBodyUpdater != null)
            {
                result = requestBodyUpdater(result);
            }
            context.Request.Body = new MemoryStream(result);
            return result;
        }

        public static string GetDefaultDomainValue(string codeUnitName)
        {
            return $"{codeUnitName.ToLower()}.test.local";
        }

        public static bool IsAPIDocumentationRequest(HttpContext context)
        {
            return context.Request.Path.ToString().StartsWith($"{ServerConfiguration.APIRoutePrefix}/{ServerConfiguration.ResourcesSubPath}/{ServerConfiguration.APISpecificationDocumentName}/");
        }
        public static int RunAPIServer<GCodeUnitSpecificCommandlineParameter, GCodeUnitSpecificConstants, GCodeUnitSpecificConfiguration>(string codeUnitName, string codeUnitDescription, Version3 codeUnitVersion, GRYEnvironment environmentTargetType, ExecutionMode executionMode, string[] commandlineArguments, Action<APIServerConfiguration<GCodeUnitSpecificConstants, GCodeUnitSpecificConfiguration, GCodeUnitSpecificCommandlineParameter>> initializer)
            where GCodeUnitSpecificConfiguration : new()
            where GCodeUnitSpecificConstants : new()
            where GCodeUnitSpecificCommandlineParameter : class, ICommandlineParameter, new()
        {
            GRYConsoleApplication<GCodeUnitSpecificCommandlineParameter, APIServerConfiguration<GCodeUnitSpecificConstants, GCodeUnitSpecificConfiguration, GCodeUnitSpecificCommandlineParameter>> consoleApp = new GRYConsoleApplication<GCodeUnitSpecificCommandlineParameter, APIServerConfiguration<GCodeUnitSpecificConstants, GCodeUnitSpecificConfiguration, GCodeUnitSpecificCommandlineParameter>>(APIServer<GCodeUnitSpecificConstants, GCodeUnitSpecificConfiguration, GCodeUnitSpecificCommandlineParameter>.APIMain, codeUnitName, codeUnitVersion.ToString(), codeUnitDescription, true, executionMode, environmentTargetType, true);
            return consoleApp.Main(commandlineArguments, initializer);
        }
        public static void ConnectToDatabase(Action connectAction, IGeneralLogger logger, string adaptedConnectionString)
        {
            bool connected = false;
            while (!connected)
            {
                try
                {
                    logger.Log($"Try to connect to database using connection-string \"{adaptedConnectionString}\".", LogLevel.Debug);
                    connectAction();
                    logger.Log($"Connected successfully.", LogLevel.Information);
                    connected = true;
                }
                catch (Exception exception)
                {
                    logger.LogException(exception, "Could not connect to database.", LogLevel.Warning);
                }
                finally
                {
                    Thread.Sleep(TimeSpan.FromSeconds(10));
                }
            }
        }
    }
}
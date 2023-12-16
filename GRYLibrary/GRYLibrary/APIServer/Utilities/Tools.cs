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

namespace GRYLibrary.Core.APIServer.Utilities
{
    public static class Tools
    {
        public static (byte[] requestBody, byte[] responsetBody) ExecuteAndGetBody(HttpContext context, Func<byte[], byte[]> responseBodyUpdater = null)
        {
            byte[] requestBody = GetRequestBody(context);
            byte[] responseBody;
            Stream originalResponseBody = context.Response.Body;
            using (MemoryStream intermediateResponseBody = new MemoryStream())
            {
                context.Response.Body = intermediateResponseBody;

                //read response body
                intermediateResponseBody.Position = 0;
                responseBody = GUtilities.StreamToByteArray(intermediateResponseBody);
                if (responseBodyUpdater != null)
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

        public static string GetDefaultDomainValue(string codeUnitName)
        {
            return $"{codeUnitName.ToLower()}.test.local";
        }

        public static byte[] GetRequestBody(HttpContext context)
        {
            context.Request.EnableBuffering();
            byte[] result = GUtilities.StreamToByteArray(context.Request.Body);
            context.Request.Body = new MemoryStream(result);
            return result;
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
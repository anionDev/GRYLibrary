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
using System.Collections.Generic;
using System.Security.Claims;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.Crypto;
using HashAlgorithm = GRYLibrary.Core.Crypto.HashAlgorithm;
using SHA256 = GRYLibrary.Core.Crypto.SHA256;
using GRYLibrary.Core.APIServer.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Linq;
using System.Threading.Tasks;
using GRYLibrary.Core.APIServer.Verbs;
using GRYLibrary.Core.Exceptions;

namespace GRYLibrary.Core.APIServer.Utilities
{
    public static class Tools
    {
        public static (byte[] requestBody, byte[] responseBody) ExecuteNextMiddlewareAndGetRequestAndResponseBody(HttpContext context, RequestDelegate next, Func<byte[], byte[]> requestBodyUpdater = null, Func<byte[], byte[]> responseBodyUpdater = null)
        {
            byte[] requestBody = GetRequestBody(context.Request, requestBodyUpdater);
            byte[] responseBody = default;
            Stream originalBody = context.Response.Body;

            using (MemoryStream memStream = new MemoryStream())
            {
                context.Response.Body = memStream;
                try
                {
                    next(context).Wait();
                }
                catch
                {
                    throw;
                }

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

        public static byte[] GetRequestBody(HttpRequest request, Func<byte[], byte[]> requestBodyUpdater = null)
        {
            request.EnableBuffering();
            byte[] result = GUtilities.StreamToByteArray(request.Body);
            if (requestBodyUpdater != null)
            {
                result = requestBodyUpdater(result);
            }
            request.Body = new MemoryStream(result);
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
            where GCodeUnitSpecificCommandlineParameter : class, IAPIServerCommandlineParameter, new()
        {
            GRYConsoleApplication<GCodeUnitSpecificCommandlineParameter> consoleApp = new GRYConsoleApplication<GCodeUnitSpecificCommandlineParameter>(new VerbParser<GCodeUnitSpecificCommandlineParameter>(APIServer<GCodeUnitSpecificConstants, GCodeUnitSpecificConfiguration, GCodeUnitSpecificCommandlineParameter>.CreateMain(initializer)), codeUnitName, codeUnitVersion.ToString(), codeUnitDescription, true, executionMode, environmentTargetType, true);
            return consoleApp.Main(commandlineArguments);
        }

        public static void ConnectToDatabase(Action connectAction, IGeneralLogger logger, string adaptedConnectionString)
        {
            ConnectToDatabase(connectAction, logger, adaptedConnectionString, TimeSpan.FromMinutes(2));
        }
        public static void ConnectToDatabase(Action connectAction, IGeneralLogger logger, string adaptedConnectionString, TimeSpan timeout)
        {
            GUtilities.RunWithTimeout(() =>
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
                        Thread.Sleep(TimeSpan.FromSeconds(2));
                    }
                }
            }, timeout);
        }

        public static bool TryGetAuthentication(ICredentialsProvider credentialsProvider, IAuthenticationService authenticationService, HttpContext context, out ClaimsPrincipal principal)
        {
            principal = default;
            try
            {
                if (credentialsProvider.ContainsCredentials(context))
                {
                    string secret = credentialsProvider.ExtractSecret(context);
                    string accessToken = secret;
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        bool accessTokenIsValid = authenticationService.AccessTokenIsValid(accessToken);
                        if (accessTokenIsValid)
                        {
                            string username = authenticationService.GetUserName(accessToken);
                            principal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new Claim(ClaimTypes.Name, username) }, "Basic"));
                            return true;
                        }
                    }
                }
            }
            catch
            {
                GUtilities.NoOperation();
            }
            return false;
        }
        private readonly static IList<HashAlgorithm> _HashAlgorithms = new List<HashAlgorithm>() { new SHA256(), new SHA256PureCSharp() };
        public static HashAlgorithm GetHashAlgorithm(string passwordHashAlgorithmIdentifier)
        {
            byte[] passwordHashAlgorithmIdentifierBytes = GUtilities.PadLeft(new UTF8Encoding(false).GetBytes(passwordHashAlgorithmIdentifier), 10);
            foreach (HashAlgorithm algorithm in _HashAlgorithms)
            {
                if (algorithm.GetIdentifier() == passwordHashAlgorithmIdentifierBytes)
                {
                    return algorithm;
                }
            }
            throw new KeyNotFoundException($"Unknown algorithm: {passwordHashAlgorithmIdentifier}");
        }
        public static void CheckService(IGeneralLogger logger, string name, IExternalService service, ref HealthStatus result, IList<string> messages, bool logIfNotAvailable, bool serviceIsRequired)
        {
            CheckService(logger, name, service == null, service.IsAvailable, ref result, messages, logIfNotAvailable, serviceIsRequired);
        }
        public static void CheckService(IGeneralLogger logger, string name, bool serviceIsNull, Func<bool> isAvailable, ref HealthStatus result, IList<string> messages, bool logIfNotAvailable, bool serviceIsRequired)
        {
            if (serviceIsNull)
            {
                string message = $"{name} is null.";
                messages.Add(message);
                if (logIfNotAvailable)
                {
                    logger.Log(message, LogLevel.Warning);
                }
                result = CalculateNotAvailableResult(result, serviceIsRequired);
                return;
            }
            if (!isAvailable())
            {
                string message = $"{name} is not available.";
                messages.Add(message);
                if (logIfNotAvailable)
                {
                    logger.Log(message, LogLevel.Warning);
                }
                result = CalculateNotAvailableResult(result, serviceIsRequired);
                return;
            }
            //more checks can be added
        }

        public static HealthStatus CalculateNotAvailableResult(HealthStatus resultUntilNow, bool dependencyIsRequired)
        {
            if (resultUntilNow == HealthStatus.Unhealthy)
            {
                return resultUntilNow;
            }
            else
            {
                if (dependencyIsRequired)
                {
                    return HealthStatus.Unhealthy;
                }
                else
                {
                    return HealthStatus.Degraded;
                }
            }
        }
#pragma warning disable IDE0060 // Remove unused parameter
        public static Task<HealthCheckResult> CheckHealthAsync(IGeneralLogger logger, Func<(HealthStatus, IEnumerable<string>)> check, HealthCheckContext context, CancellationToken cancellationToken)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            (HealthStatus result, IEnumerable<string> messages) result;
            try
            {
                result = check();
            }
            catch (Exception exception)
            {
                result = (HealthStatus.Unhealthy, new List<string>() { GUtilities.GetExceptionMessage(exception, "Error while calculating health-status", true) });
            }
            int messageCount = result.messages.Count();
            LogLevel loglevel;
            string message;
            if (result.result.Equals(HealthStatus.Healthy))
            {
                message = "Service is healthy.";
                loglevel = LogLevel.Debug;
            }
            else if (result.result.Equals(HealthStatus.Degraded))
            {
                message = "Service is degraded.";
                loglevel = LogLevel.Debug;
            }
            else if (result.result.Equals(HealthStatus.Unhealthy))
            {
                message = "Service is unhealthy.";
                loglevel = LogLevel.Warning;
            }
            else
            {
                throw new InternalAlgorithmException($"Unknown healthstatus: {(int)result.result}");
            }

            if (messageCount > 0)
            {
                message = $"{message} The following messages occurred: " + string.Join(", ", result.messages.Select(message => "\"" + message + "\""));
            }
            logger.Log(message, loglevel);
            HealthCheckResult healthCheckResult;
            if (result.result.Equals(HealthStatus.Healthy))
            {
                healthCheckResult = HealthCheckResult.Healthy(message);
            }
            else if (result.result.Equals(HealthStatus.Degraded))
            {
                healthCheckResult = HealthCheckResult.Degraded(message);
            }
            else if (result.result.Equals(HealthStatus.Unhealthy))
            {
                healthCheckResult = HealthCheckResult.Unhealthy(message);
            }
            else
            {
                throw new InternalAlgorithmException($"Undknown healthstatus: {(int)result.result}");
            }
            return Task.FromResult(healthCheckResult);
        }
    }
}
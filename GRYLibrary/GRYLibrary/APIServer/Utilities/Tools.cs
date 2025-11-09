using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.ConcreteEnvironments;
using GRYLibrary.Core.APIServer.ExecutionModes;
using GRYLibrary.Core.APIServer.Services;
using GRYLibrary.Core.APIServer.Services.Init;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Settings;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.APIServer.Utilities.InitializationStates;
using GRYLibrary.Core.APIServer.Verbs;
using GRYLibrary.Core.Crypto;
using GRYLibrary.Core.Exceptions;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using GRYLibrary.Core.Misc;
using GRYLibrary.Core.Misc.ConsoleApplication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GUtilities = GRYLibrary.Core.Misc.Utilities;
using HashAlgorithm = GRYLibrary.Core.Crypto.HashAlgorithm;
using SHA256 = GRYLibrary.Core.Crypto.SHA256;

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

        public static bool IsMaintenanceRequest(HttpContext context)
        {
            return context.Request.Path.ToString().StartsWith($"{ServerConfiguration.APIRoutePrefix}/Other/Maintenance/");
        }

        public static int RunAPIServer<GCodeUnitSpecificCommandlineParameter, GCodeUnitSpecificConstants, GCodeUnitSpecificConfiguration>(string codeUnitName, string codeUnitDescription, Version3 codeUnitVersion, GRYEnvironment environmentTargetType, ExecutionMode executionMode, string[] commandlineArguments, string? additionalHelpText, Action<APIServerConfiguration<GCodeUnitSpecificConstants, GCodeUnitSpecificConfiguration, GCodeUnitSpecificCommandlineParameter>> initializer)
            where GCodeUnitSpecificConfiguration : new()
            where GCodeUnitSpecificConstants : new()
            where GCodeUnitSpecificCommandlineParameter : class, IAPIServerCommandlineParameter, new()
        {
            int exitCode = 0;
            Exception? exception = null;//for debugging-purposes
            try
            {
                GRYConsoleApplication<GCodeUnitSpecificCommandlineParameter> consoleApp = new GRYConsoleApplication<GCodeUnitSpecificCommandlineParameter>(new VerbParser<GCodeUnitSpecificCommandlineParameter>(APIServer<GCodeUnitSpecificConstants, GCodeUnitSpecificConfiguration, GCodeUnitSpecificCommandlineParameter>.CreateMain(initializer)), codeUnitName, codeUnitVersion.ToString(), codeUnitDescription, true, executionMode, environmentTargetType, additionalHelpText);
                exitCode = consoleApp.Main(commandlineArguments);
            }
            catch (Exception ex)
            {
#pragma warning disable IDE0059 // Unnecessary assignment of a value
                exception = ex;
#pragma warning restore IDE0059 // Unnecessary assignment of a value
                exitCode = 1;
            }
            if (exitCode != 0 && Debugger.IsAttached)
            {
                Console.ReadLine();
            }
            return exitCode;
        }

        public static void WaitUntilDatabaseIsAvailable(Func<bool> databaseIsAvailableCheck, IGeneralLogger logger,uint maximalAmountOfAttempts=25, uint initialAmountOfSecondsToWait = 0)
        {
            bool isAvailable;
            try
            {
                isAvailable = databaseIsAvailableCheck();//check if the database is already available
            }
            catch
            {
                isAvailable = false;
            }
            uint amoutnOfFails = 0;
            if (!isAvailable)
            {
                Thread.Sleep(TimeSpan.FromSeconds(initialAmountOfSecondsToWait));
                while (!isAvailable)
                {
                    try
                    {
                        isAvailable = databaseIsAvailableCheck();
                    }
                    catch (Exception ex)
                    {
                        amoutnOfFails = amoutnOfFails + 1;
                        if (amoutnOfFails== maximalAmountOfAttempts)
                        {
                            throw new DependencyNotAvailableException("Database not available.", ex);
                        }
                        logger.Log("Database not available yet.", ex, LogLevel.Warning);
                        Thread.Sleep(TimeSpan.FromSeconds(5));
                    }
                }
            }
            logger.Log("Database is now available.", LogLevel.Information);
        }
        
        public static bool TryGetAuthentication(ICredentialsProvider credentialsProvider, HttpContext context, out string accessToken)
        {
            try
            {
                if (credentialsProvider.ContainsCredentials(context))
                {
                    string secret = credentialsProvider.ExtractSecret(context);
                    if (!string.IsNullOrEmpty(secret))
                    {
                        accessToken = secret;
                        return true;
                    }
                }
            }
            catch
            {
                GUtilities.NoOperation();
            }
            accessToken = null;
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
        public static Task<HealthCheckResult> CheckHealthAsync(IGeneralLogger logger, Func<(HealthStatus, IEnumerable<string>)> check, HealthCheckContext context, CancellationToken cancellationToken, IInitializationService? initializationService)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            (HealthStatus result, IEnumerable<string> messages) result;
            try
            {
                if (initializationService != null && initializationService.GetInitializationState().Equals(new InitializationFailed()))
                {
                    result = (HealthStatus.Degraded, new List<string>() { "Initializing" });
                }
                else
                {
                    result = check();
                }
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
                loglevel = LogLevel.Warning;
            }
            else if (result.result.Equals(HealthStatus.Unhealthy))
            {
                message = "Service is unhealthy.";
                loglevel = LogLevel.Error;
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
        public static User GetUser(ClaimsPrincipal principal, IAuthenticationService authenticationService)
        {
            User result = authenticationService.GetUser(principal.Claims.Where(claim => claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").First().Value);
            return result;
        }
    }
}
﻿using GRYLibrary.Core.APIServer.MidT.Exception;
using GRYLibrary.Core.Exceptions;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.Mid.Ex
{
    public class DefaultExceptionHandlerMiddleware : ExceptionManagerMiddleware
    {
        private readonly IGeneralLogger _GeneralLogger;
        public DefaultExceptionHandlerMiddleware(RequestDelegate next, IGeneralLogger logger) : base(next)
        {
            this._GeneralLogger = logger;
        }

        protected async override Task HandleException(HttpContext context, Exception exception)
        {
            Exception exceptionForFormatting;
            if (exception == null)
            {
                throw new NotImplementedException();
            }
            else if (exception is AggregateException aggregateException2)
            {
                if (aggregateException2.InnerExceptions == null)
                {
                    throw new NotImplementedException();
                }
                else
                {
                    if (aggregateException2.InnerExceptions.Count == 0)
                    {
                        throw new NotImplementedException();
                    }
                    else if (aggregateException2.InnerExceptions.Count == 1)
                    {
                        exceptionForFormatting = aggregateException2.InnerExceptions[0];
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
            }
            else
            {
                exceptionForFormatting = exception;
            }
            if (exceptionForFormatting is BadRequestException badHttpRequestException)
            {
                context.Response.StatusCode = (int)badHttpRequestException.HTTPStatusCode;
            }
            else
            {
                this._GeneralLogger.LogException(exceptionForFormatting, "Error while processing request");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            (string ContentType, string bodyContent) = this.GetResonceContent(context.Response.StatusCode, context, exceptionForFormatting);
            context.Response.ContentType = ContentType;
            await context.Response.WriteAsync(bodyContent);
            return;
        }
        public virtual (string ContentType, string bodyContent) GetResonceContent(int httpStatusCode, HttpContext context, Exception exception)
        {
            return (null, string.Empty);
        }
    }
}

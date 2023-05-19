using GRYLibrary.Core.GenericWebAPIServer.Middlewares.MiddlewareConfigurationInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.ConcreteMiddlewares
{
    public class CaptchaMiddleware :AbstractMiddleware
    {
        private readonly ICaptchaMiddlewareSettings _CaptchaMiddlewareSettings;
        /// <inheritdoc/>
        public CaptchaMiddleware(RequestDelegate next, ICaptchaMiddlewareSettings captchaMiddlewareSettings) : base(next)
        {
            this._CaptchaMiddlewareSettings = captchaMiddlewareSettings;
        }
        /// <inheritdoc/>
        public override Task Invoke(HttpContext context)
        {
            if(this.UserHasAlreadySolvedTheCaptcha(_CaptchaMiddlewareSettings, context))
            {
                return this._Next(context);
            }
            else
            {
                HttpStatusCode statusCode = HttpStatusCode.OK;
                string message = string.Empty;
                string captchaIdKey = "captchaId";
                string captchaValueKey = "captchaValue";
                if(this.UserTriesToSolveCaptcha(context))
                {
                    string captchaId = context.Request.Query[captchaIdKey];
                    string captchaValue = context.Request.Query[captchaValueKey];
                    bool result = this._CaptchaMiddlewareSettings.TrySolve(captchaId, captchaValue, out string accessKey, out string failMessage);
                    if(result)
                    {
                        List<KeyValuePair<string, string>> query = context.Request.Query.SelectMany(x => x.Value, (col, value) => new KeyValuePair<string, string>(col.Key, value)).ToList();
                        query.RemoveAll(queryParameter => queryParameter.Key == captchaIdKey || queryParameter.Key == captchaValueKey);
                        context.Request.QueryString = new QueryBuilder(query).ToQueryString();
                        this.SetAccessToken(_CaptchaMiddlewareSettings, context, accessKey);
                        context.Response.Redirect(context.Request.Path);
                        return Task.CompletedTask;
                    }
                    else
                    {
                        statusCode = HttpStatusCode.BadRequest;
                        message = failMessage;
                    }
                    //TODO if too many failed requests so that the user should be blocked (maybe in the context of the Blacklist-middleware) send an unreasolvable captcha whose text ist always something like "blocked".
                }
                System.Text.Encoding encoding = Miscellaneous.Utilities.GetEncodingByIdentifier(this._CaptchaMiddlewareSettings.Encoding);
                (string id, byte[] picture) = this._CaptchaMiddlewareSettings.GenerateCaptcha();
                string captchaBase64 = Convert.ToBase64String(picture);
                context.Response.StatusCode = (int)statusCode;
                context.Response.ContentType = "text/html";
                string html = _CaptchaMiddlewareSettings.CaptchaPage;
                html = html.Replace($"__message__", message);
                html = html.Replace($"__captchabase64__", captchaBase64);
                html = html.Replace($"__captchaid__", id);
                html = html.Replace($"__captchaidquerykey__", captchaIdKey);
                html = html.Replace($"__captchavaluequerykey__", captchaValueKey);
                context.Response.BodyWriter.WriteAsync(encoding.GetBytes(html));
                return Task.CompletedTask;
            }
        }

        private bool UserTriesToSolveCaptcha(HttpContext context)
        {
            return context.Request.Query.ContainsKey("captchaId") && context.Request.Query.ContainsKey("captchaValue");
        }

        private bool UserHasAlreadySolvedTheCaptcha(ICaptchaMiddlewareSettings settings, HttpContext context)
        {
            return this._CaptchaMiddlewareSettings.UserHasAlreadySolvedTheCaptcha(this.GetAccessToken(settings, context), out string failMessage);
        }

        private void SetAccessToken(ICaptchaMiddlewareSettings settings, HttpContext context, string accessKey)
        {
            context.Response.Cookies.Append(settings.CaptchaCookieName, accessKey);
        }

        private string GetAccessToken(ICaptchaMiddlewareSettings settings, HttpContext context)
        {
            return context.Request.Cookies.Where(c => c.Key == settings.CaptchaCookieName).FirstOrDefault().Value;
        }
    }
}
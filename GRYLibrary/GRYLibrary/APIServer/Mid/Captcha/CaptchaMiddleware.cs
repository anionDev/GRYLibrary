﻿using GRYLibrary.Core.Miscellaneous;
using GRYLibrary.Core.Miscellaneous.Captcha;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.Mid.Captcha
{
    public class CaptchaMiddleware :AbstractMiddleware
    {
        private readonly CaptchaManager _CaptchaManager;
        private readonly ICaptchaConfiguration _CaptchaMiddlewareSettings;
        /// <inheritdoc/>
        public CaptchaMiddleware(RequestDelegate next, ICaptchaConfiguration captchaMiddlewareSettings) : base(next)
        {
            this._CaptchaManager = CaptchaManager.Instance;
            this._CaptchaMiddlewareSettings = captchaMiddlewareSettings;
        }
        /// <inheritdoc/>
        public override Task Invoke(HttpContext context)
        {
            if(this.UserHasAlreadySolvedTheCaptcha(this._CaptchaMiddlewareSettings, context))
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
                    bool result = this.TrySolve(captchaId, captchaValue, out string accessKey, out string failMessage);
                    if(result)
                    {
                        List<KeyValuePair<string, string>> query = context.Request.Query.SelectMany(x => x.Value, (col, value) => new KeyValuePair<string, string>(col.Key, value)).ToList();
                        query.RemoveAll(queryParameter => queryParameter.Key == captchaIdKey || queryParameter.Key == captchaValueKey);
                        context.Request.QueryString = new QueryBuilder(query).ToQueryString();
                        this.SetAccessToken(this._CaptchaMiddlewareSettings, context, accessKey);
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
                (string id, byte[] picture) = this.GenerateCaptcha();
                string captchaBase64 = Convert.ToBase64String(picture);
                context.Response.StatusCode = (int)statusCode;
                context.Response.ContentType = "text/html";
                string html = this._CaptchaMiddlewareSettings.CaptchaPage;
                IDictionary<string, string> replacements = new Dictionary<string, string>
                {
                    { "message", message },
                    { "captchabase64", captchaBase64 },
                    { "captchaid", id },
                    { "captchaidquerykey", captchaIdKey },
                    { "captchavaluequerykey", captchaValueKey }
                };
                IDictionary< string, bool> booleanReplacements = new Dictionary<string, bool>(); 
                IDictionary<string, Func<string>> variables = new Dictionary<string, Func<string>>();
                ReplacementTools.ReplaceVariables(html, replacements, booleanReplacements, variables);
                context.Response.BodyWriter.WriteAsync(encoding.GetBytes(html));
                return Task.CompletedTask;
            }
        }

        private bool UserTriesToSolveCaptcha(HttpContext context)
        {
            return context.Request.Query.ContainsKey("captchaId") && context.Request.Query.ContainsKey("captchaValue");
        }

        private bool UserHasAlreadySolvedTheCaptcha(ICaptchaConfiguration settings, HttpContext context)
        {
            return this.UserHasAlreadySolvedTheCaptcha(this.GetAccessToken(settings, context), out string failMessage);
        }

        private void SetAccessToken(ICaptchaConfiguration settings, HttpContext context, string accessKey)
        {
            context.Response.Cookies.Append(settings.CaptchaCookieName, accessKey);
        }

        private string GetAccessToken(ICaptchaConfiguration settings, HttpContext context)
        {
            return context.Request.Cookies.Where(c => c.Key == settings.CaptchaCookieName).FirstOrDefault().Value;
        }
        public bool TrySolve(string captchaId, string userInput, out string accessKey, out string failMessage)
        {
            return this._CaptchaManager.TrySolve(captchaId, userInput, out accessKey, out failMessage);
        }
        public (string id, byte[] picture) GenerateCaptcha()
        {
            CaptchaInstance result = this._CaptchaManager.GetNewCaptcha(this.GetCaptchaSettings());
            return (result.Id, result.PictureContent);
        }

        private CaptchaGenerationSettings GetCaptchaSettings()
        {
            CaptchaGenerationSettings result = new CaptchaGenerationSettings()
            {
                Alphabet = this._CaptchaMiddlewareSettings. Alphabet,
                ExpireDurationOfCaptcha = this._CaptchaMiddlewareSettings.ExpireDurationOfCaptcha,
                Length = this._CaptchaMiddlewareSettings.Length,
                ExpireDurationOfAccessToken = this._CaptchaMiddlewareSettings.ExpireDurationOfAccessToken
            };
            return result;
        }

        public bool UserHasAlreadySolvedTheCaptcha(string accessToken, out string failMessage)
        {
            return this._CaptchaManager.UserHasAlreadySolvedTheCaptcha(accessToken, out failMessage);
        }
    }
}
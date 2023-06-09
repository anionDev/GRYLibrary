﻿using GRYLibrary.Core.GenericWebAPIServer.Middlewares;
using System;

namespace GRYLibrary.Core.GenericWebAPIServer.DefinedMiddlewares.Captcha
{
    public interface ICaptchaConfiguration :IMiddlewareConfiguration
    {
        public string Encoding { get; set; }
        public ushort Length { get; set; }
        public string Alphabet { get; set; }
        public TimeSpan ExpireDurationOfCaptcha { get; set; }
        public TimeSpan ExpireDurationOfAccessToken { get; set; }
        public string CaptchaCookieName { get; set; }
        public string CaptchaPage { get; set; }
    }
}

﻿namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares
{
    public interface IMiddlewareSettings
    {
        public bool Enabled { get; set; }
        string CaptchaCookieName { get; set; }
    }
}
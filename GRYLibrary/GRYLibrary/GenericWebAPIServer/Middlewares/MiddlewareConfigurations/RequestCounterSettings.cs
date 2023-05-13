﻿using GRYLibrary.Core.GenericWebAPIServer.Middlewares.MiddlewareConfigurationInterfaces;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.MiddlewareConfigurations
{
    public class RequestCounterSettings :IRequestCounterSettings
    {
        public bool Enabled { get; set; } = false;
    }
}
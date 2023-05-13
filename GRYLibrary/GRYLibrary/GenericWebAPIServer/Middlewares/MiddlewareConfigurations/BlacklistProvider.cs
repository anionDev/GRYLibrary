﻿using GRYLibrary.Core.GenericWebAPIServer.Middlewares.MiddlewareConfigurationInterfaces;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.MiddlewareConfigurations
{
    public class BlacklistProvider :IBlacklistProvider
    {
        public bool Enabled { get; set; } = false;
 
    }
}
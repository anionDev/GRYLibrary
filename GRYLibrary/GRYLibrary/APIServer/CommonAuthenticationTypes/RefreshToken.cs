﻿using System;

namespace GRYLibrary.Core.APIServer.CommonAuthenticationTypes
{
    public class RefreshToken
    {
        public string Value { get; set; }
        public DateTime ExpiredMoment { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.Utilities
{
    public interface IBackgroundService
    {
        public void StartAsync();
        public Task Stop();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.Miscellaneous.Healthcheck
{
    public enum HealthcheckValue
    {
        NotRunning = 0,
        RunningHealthy = 1,
        RunningUnhealthy = 2,
    }
}

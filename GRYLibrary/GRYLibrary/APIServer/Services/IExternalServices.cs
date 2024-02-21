using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.Services
{

    public interface IExternalService
    {
        public bool IsAvailable();
    }
}

using System;

namespace GRYLibrary.Core.APIServer.Services
{
    public interface IExternalService : IDisposable
    {
        public bool IsAvailable();
    }
}

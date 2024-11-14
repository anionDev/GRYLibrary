namespace GRYLibrary.Core.APIServer.Services.Aut.Prov.OpenId
{
    public class OpenIdProvider : IOpenIdProvider
    {
        private readonly IOpenIdConfiguration _Configuration;
        public OpenIdProvider(IOpenIdConfiguration configuration)
        {
            this._Configuration = configuration;
        }
    }
}

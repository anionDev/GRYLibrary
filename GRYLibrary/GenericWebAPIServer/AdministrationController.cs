using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GRYLibrary.Core.GenericWebAPIServer
{
    [Route("Administration")]
    public class AdministrationController : ControllerBase
    {
        private readonly IAdministrationSettings _AdministrationSettings;
        private readonly ISettingsInterface _Settings;
        public AdministrationController(IAdministrationSettings administrationSettings, ISettingsInterface settings)
        {
            this._AdministrationSettings = administrationSettings;
            this._Settings = settings;
        }

        [HttpGet]
        [Route("/" + nameof(Version))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public IActionResult Version()
        {
            if (_Settings.ProgramVersionIsQueryable || !(_AdministrationSettings.Environment is Productive))
            {
                return this.StatusCode(StatusCodes.Status200OK, _AdministrationSettings.Version.ToString());
            }
            else
            {
                return this.StatusCode(StatusCodes.Status406NotAcceptable);
            }
        }
    }
}

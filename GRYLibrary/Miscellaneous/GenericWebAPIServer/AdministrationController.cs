using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace GRYLibrary.Core.Miscellaneous.GenericWebAPIServer
{
    [Route("Administration")]
    public class AdministrationController : ControllerBase
    {
        private readonly IAdministrationSettings _AdministrationSettings;
        private readonly ISettingsInterface _SettingsInterface;
        public AdministrationController(IAdministrationSettings administrationSettings, ISettingsInterface settings)
        {
            this._AdministrationSettings = administrationSettings;
            this._SettingsInterface = settings;
        }

        [HttpGet]
        [Route("/" + nameof(Version))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public IActionResult Version()
        {
            if (_SettingsInterface.ProgramVersionIsQueryable)
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

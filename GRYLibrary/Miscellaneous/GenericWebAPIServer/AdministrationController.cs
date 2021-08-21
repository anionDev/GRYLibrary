using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        [ProducesResponseType(StatusCodes.Status406)]
        public string Version()
        {
            if (_SettingsInterface.ProgramVersionIsQueryable)
            {
                return _AdministrationSettings.Version.ToString();
            }
            else
            {
                return this.StatusCode(StatusCodes.Status406);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.Services.GDPR
{
  public interface IGDPRService
    {
        public ISet<PersonalData> GetPersonalData(string personIdentifier);
        public void DeleteDeletablePersonalData(string personIdentifier);
    }
}

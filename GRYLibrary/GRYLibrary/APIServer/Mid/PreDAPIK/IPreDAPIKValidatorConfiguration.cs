using GRYLibrary.Core.APIServer.Mid.NewFolder;
using System.Collections.Generic;

namespace GRYLibrary.Core.APIServer.Mid.PreDAPIK
{
    public interface IPreDAPIKValidatorConfiguration : IAPIKeyValidatorConfiguration
    {
        public IList<string> AuthorizedAPIKeys { get; set; }
    }
}

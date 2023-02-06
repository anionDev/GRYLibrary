using GRYLibrary.Core.Miscellaneous.MetaConfiguration.ConfigurationFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.Miscellaneous.MetaConfiguration
{
    public class MetaConfigurationSettings<T, TBase> where T : TBase, new()
    {
        public IConfigurationFormat ConfigurationFormat{ get; set; }
        public T InitialValue { get; set; }
        public string File { get; set; }

    }
}

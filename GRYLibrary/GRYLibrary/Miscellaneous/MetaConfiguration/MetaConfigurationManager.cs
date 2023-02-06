using GRYLibrary.Core.Miscellaneous.MetaConfiguration.ConfigurationFormats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.Miscellaneous.MetaConfiguration
{
    public class MetaConfigurationManager
    {
        public static T GetConfiguration<T, TBase>(MetaConfigurationSettings<T, TBase> configuration) where T : TBase, new()
        {
            return configuration.ConfigurationFormat.Accept(new HandleConfigurationVisitor<T, TBase>(configuration));
        }
        private class HandleConfigurationVisitor<T, TBase> : IConfigurationFormatVisitor<T> where T : TBase, new()
        {
            private readonly MetaConfigurationSettings<T, TBase> _Configuration;

            public HandleConfigurationVisitor(MetaConfigurationSettings<T, TBase> configuration)
            {
                this._Configuration = configuration;
            }

            public T Handle(XML xML)
            {
                return Utilities.CreateOrLoadXMLConfigurationFile<T, TBase>(_Configuration.File, _Configuration.InitialValue);
            }

            public T Handle(JSON jSON)
            {
                return Utilities.CreateOrLoadJSONConfigurationFile<T, TBase>(_Configuration.File, _Configuration.InitialValue);
            }
        }
    }
}

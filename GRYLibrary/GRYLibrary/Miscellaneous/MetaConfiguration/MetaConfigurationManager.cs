using GRYLibrary.Core.Miscellaneous.MetaConfiguration.ConfigurationFormats;
using System.Collections.Generic;
using System;

namespace GRYLibrary.Core.Miscellaneous.MetaConfiguration
{
    public class MetaConfigurationManager
    {
        public static T GetConfiguration<T, TBase>(MetaConfigurationSettings<T, TBase> configuration, ISet<Type> knownTypes) where T : TBase, new()
        {
            //TODO run migration from MetaConfigurationSettings here if required
            return configuration.ConfigurationFormat.Accept(new HandleConfigurationVisitor<T, TBase>(configuration, knownTypes));
        }
        private class HandleConfigurationVisitor<T, TBase> : IConfigurationFormatVisitor<T> where T : TBase, new()
        {
            private readonly MetaConfigurationSettings<T, TBase> _Configuration;
            private readonly ISet<Type> _KnownTypes;
            public HandleConfigurationVisitor(MetaConfigurationSettings<T, TBase> configuration, ISet<Type> knownTypes)
            {
                this._Configuration = configuration;
                this._KnownTypes = knownTypes;
            }

            public T Handle(XML xML)
            {
                return Utilities.CreateOrLoadXMLConfigurationFile<T, TBase>(this._Configuration.File, this._Configuration.InitialValue, this._KnownTypes);
            }

            public T Handle(JSON jSON)
            {
                return Utilities.CreateOrLoadJSONConfigurationFile<T, TBase>(this._Configuration.File, this._Configuration.InitialValue);
            }
        }
    }
}
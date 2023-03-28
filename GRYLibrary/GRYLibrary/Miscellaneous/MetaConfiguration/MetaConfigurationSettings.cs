using GRYLibrary.Core.Miscellaneous.MetaConfiguration.ConfigurationFormats;

namespace GRYLibrary.Core.Miscellaneous.MetaConfiguration
{
    public class MetaConfigurationSettings<T, TBase> where T : TBase, new()
    {
        public IConfigurationFormat ConfigurationFormat { get; set; }
        public T InitialValue { get; set; }
        public string File { get; set; }
    }
}
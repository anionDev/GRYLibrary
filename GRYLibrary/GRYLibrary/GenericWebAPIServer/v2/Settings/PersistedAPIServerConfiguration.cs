using GRYLibrary.Core.Miscellaneous.FilePath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.v2.Settings
{
    /// <summary>
    /// Represents Application-constants which are editable by a configuration-file.
    /// </summary>
    public interface IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration>
        where PersistedApplicationSpecificConfiguration : new()
    {
        public PersistedApplicationSpecificConfiguration ApplicationSpecificConfiguration { get; set; }
        public WebServerConfiguration WebServerConfiguration { get; set; }
    }
    public class PersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration> :IPersistedAPIServerConfiguration<PersistedApplicationSpecificConfiguration>
        where PersistedApplicationSpecificConfiguration : new()
    {
        public PersistedApplicationSpecificConfiguration ApplicationSpecificConfiguration { get; set; }
        public WebServerConfiguration WebServerConfiguration { get; set; }
    }
}

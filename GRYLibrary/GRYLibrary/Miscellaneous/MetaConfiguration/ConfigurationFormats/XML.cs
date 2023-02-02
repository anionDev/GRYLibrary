using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.Miscellaneous.MetaConfiguration.ConfigurationFormats
{
    public class XML : IConfigurationFormat
    {
        public static XML Instance { get; } = new XML();
        private XML() { }
        public void Accept(IConfigurationFormatVisitor visitor)
        {
            visitor.Handle(this);
        }

        public T Accept<T>(IConfigurationFormatVisitor<T> visitor)
        {
            return visitor.Handle(this);
        }
    }
}

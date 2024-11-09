using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.Settings
{
    public abstract class Regulation
    {
        public virtual IList<Type> RequiredServices { get; set; } = new List<Type>();//TODO enforce this service
    }
}

using System;
using System.Collections.Generic;

namespace GRYLibrary.Core.APIServer.CommonDBTypes
{
    public class UserGroup
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public ISet<Guid> UserIds { get; set; } = new HashSet<Guid>();
    }
}

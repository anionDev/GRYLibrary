using System;
using System.Collections.Generic;

namespace GRYLibrary.Core.APIServer.CommonDBTypes
{
    public class UserGroup
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public ISet<string> UserIds { get; set; } = new HashSet<string>();
    }
}

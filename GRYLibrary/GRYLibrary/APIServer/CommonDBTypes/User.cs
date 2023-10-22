using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace GRYLibrary.Core.APIServer.CommonDBTypes
{
    [PrimaryKey(nameof(Id))]
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public ISet<string> Groups { get; set; } = new HashSet<string>();
    }
}

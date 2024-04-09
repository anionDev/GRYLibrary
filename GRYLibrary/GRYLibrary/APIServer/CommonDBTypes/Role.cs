using Microsoft.EntityFrameworkCore;
using System;

namespace GRYLibrary.Core.APIServer.CommonDBTypes
{
    [PrimaryKey(nameof(Id))]
    public class Role : IEquatable<Role>
    {
        public string Id { get; set; }
        public string Name { get; set; } = null;

        public override bool Equals(object obj)
        {
            return this.Equals(obj as User);
        }

        public bool Equals(Role other)
        {
            return other is not null &&
                   this.Id == other.Id &&
                   this.Name == other.Name;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(this.Id);
            return hash.ToHashCode();
        }
    }
}

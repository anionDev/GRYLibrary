using GRYLibrary.Core.APIServer.CommonAuthenticationTypes;
using GRYLibrary.Core.APIServer.MFA;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace GRYLibrary.Core.APIServer.CommonDBTypes
{
    [PrimaryKey(nameof(Id))]
    public class User : IEquatable<User>
    {
        public string Id { get; set; }
        public string Name { get; set; } = null;
        public string PasswordHash { get; set; }
        public string EMailAddress { get; set; } = null;
        public bool UserIsActivated { get; set; } = true;
        public bool UserIsLocked { get; set; } = false;
        public ISet<RefreshToken> RefreshToken { get; set; } = new HashSet<RefreshToken>();
        public ISet<AccessToken> AccessToken { get; set; } = new HashSet<AccessToken>();
        public ISet<IMFAMethod> MFAMethods { get; set; } = new HashSet<IMFAMethod>();

        public override bool Equals(object obj)
        {
            return this.Equals(obj as User);
        }

        public bool Equals(User other)
        {
            return other is not null &&
                   this.Id == other.Id &&
                   this.Name == other.Name &&
                   this.EMailAddress == other.EMailAddress &&
                   this.PasswordHash == other.PasswordHash &&
                   this.UserIsActivated == other.UserIsActivated &&
                   this.UserIsLocked == other.UserIsLocked &&
                   this.RefreshToken.SetEquals(other.RefreshToken) &&
                   this.AccessToken.Equals(other.AccessToken) &&
                   this.MFAMethods.Equals(other.MFAMethods);
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(this.Id);
            return hash.ToHashCode();
        }
    }
}

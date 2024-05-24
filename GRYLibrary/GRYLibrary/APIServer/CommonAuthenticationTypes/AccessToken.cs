using Microsoft.EntityFrameworkCore;
using System;

namespace GRYLibrary.Core.APIServer.CommonAuthenticationTypes
{
    [PrimaryKey(nameof(Value))]
    public class AccessToken
    {
        public string Value { get; set; }
        public DateTime ExpiredMoment { get; set; }
    }
}

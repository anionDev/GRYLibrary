using GRYLibrary.Core.APIServer.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace GRYLibrary.Core.APIServer.CommonAuthenticationTypes
{
    [PrimaryKey(nameof(Value))]
    public class AccessToken
    {
        public string OwnerUserId { get; set; }
        public string Value { get; set; }
        public DateTime ExpiredMoment { get; set; }
        public bool IsValid(ITimeService timeService)
        {
            return timeService.GetCurrentTime().ToUniversalTime() < ExpiredMoment.ToUniversalTime();
        }
    }
}

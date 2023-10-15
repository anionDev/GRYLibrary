using Microsoft.EntityFrameworkCore;
using System;

namespace GRYLibrary.Core.APIServer.CommonDBTypes
{
    [PrimaryKey(nameof(APIKeyValue))]
    public class APIKey
    {
        public string APIKeyValue { get; set; }
        public DateTime ExpireMoment { get; set; }
        public string Description { get; set; }
    }
}

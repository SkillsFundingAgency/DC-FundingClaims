using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ESFA.DC.FundingClaims.ReminderService.Configuration
{
    class ConnectionStrings
    {
        [JsonRequired]
        public string AppLogs { get; set; }

        [JsonRequired]
        public string FundingClaims { get; set; }

        [JsonRequired]
        public string JobManagement { get; set; }
    }
}

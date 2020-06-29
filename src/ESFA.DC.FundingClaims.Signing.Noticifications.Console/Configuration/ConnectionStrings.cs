using Newtonsoft.Json;

namespace ESFA.DC.FundingClaims.Signing.Noticifications.Console.Configuration
{
    class ConnectionStrings
    {
        [JsonRequired]
        public string AppLogs { get; set; }

        [JsonRequired]
        public string FundingClaims { get; set; }


        [JsonRequired]
        public string JobManagement_RO { get; set; }
    }
}

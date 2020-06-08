using Newtonsoft.Json;

namespace ESFA.DC.FundingClaim.Console.Settings
{
    public class ConnectionStrings
    {
        [JsonRequired]
        public string AppLogs { get; set; }

        [JsonRequired]
        public string DCFTFundingClaimsData { get; set; }

        [JsonRequired]
        public string FundingClaims { get; set; }
    }
}

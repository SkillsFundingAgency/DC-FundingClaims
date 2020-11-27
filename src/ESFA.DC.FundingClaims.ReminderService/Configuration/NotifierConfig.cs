using ESFA.DC.FundingClaims.EmailNotification.Services;
using Newtonsoft.Json;

namespace ESFA.DC.FundingClaims.ReminderService.Configuration
{
    public class NotifierConfig : INotifierConfig
    {
        [JsonRequired]
        public string ApiKey { get; set; }
    }
}

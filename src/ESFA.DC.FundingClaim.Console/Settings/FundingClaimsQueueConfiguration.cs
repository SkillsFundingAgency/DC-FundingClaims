using System;
using Newtonsoft.Json;

namespace ESFA.DC.FundingClaim.Console.Settings
{
    public class FundingClaimsQueueConfiguration : ESFA.DC.Queueing.Interface.Configuration.IQueueConfiguration
    {
        [JsonRequired]
        public string ConnectionString { get; set; }

        [JsonRequired]
        public string QueueName { get; set; }

        public int MaxConcurrentCalls => 1;

        public int MinimumBackoffSeconds => 5;

        public int MaximumBackoffSeconds => 50;

        public int MaximumRetryCount => 3;

        public TimeSpan MaximumCallbackTimeSpan => TimeSpan.FromMinutes(10);
    }
}

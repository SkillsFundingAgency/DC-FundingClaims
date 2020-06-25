using System;

namespace ESFA.DC.FundingClaims.Data.Entities
{
    public partial class SigningNotificationFeed
    {
        public int Id { get; set; }

        public DateTime FeedDateTimeUtc { get; set; }

        public string LatestFeedUri { get; set; }

        public string SyndicationFeedId { get; set; }
    }
}

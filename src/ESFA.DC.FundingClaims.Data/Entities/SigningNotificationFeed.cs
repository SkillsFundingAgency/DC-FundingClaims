using System;

namespace ESFA.DC.FundingClaims.Data.Entities
{
    public partial class SigningNotificationFeed
    {
        public int Id { get; set; }

        public DateTime FeedDateTimeUtc { get; set; }

        public int PageNumber { get; set; }

        public Guid SyndicationFeedId { get; set; }

        public DateTime DateTimeUpdatedUtc { get; set; }
    }
}

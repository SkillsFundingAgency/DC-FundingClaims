using System;

namespace ESFA.DC.FundingClaims.Data.Entities
{
    public partial class SigningNotificationFeed
    {
        public int Id { get; set; }

        public DateTime UpdatedDateTimeUtc { get; set; }

        public Guid SyndicationFeedId { get; set; }

        public int PageNumber { get; set; }
    }
}

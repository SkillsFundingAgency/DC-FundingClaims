using System;
using System.Collections.Generic;

namespace ESFA.DC.FundingClaims.Signing.Models
{
    public class FundingClaimsNotificationFeed
    {
        public FundingClaimsNotificationFeed()
        {
            FundingClaimsNotificationFeedEntries = new List<FundingClaimsNotificationFeedData>();
        }

        public Guid? UuId { get; set; }

        public string FeedId { get; set; }

        public string Title { get; set; }

        public DateTime? FeedUpdated { get; set; }

        public string Author { get; set; }

        public string PreviousArchive { get; set; }

        public string NextArchive { get; set; }

        public DateTime InsertDate { get; set; }

        public int FundingClaimsNotificationFeedXmlId { get; set; }

        public ICollection<FundingClaimsNotificationFeedData> FundingClaimsNotificationFeedEntries { get; set; }

    }
}

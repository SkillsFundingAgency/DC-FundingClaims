using System;

namespace ESFA.DC.FundingClaims.Data.Entities
{
    public partial class FundingClaimsSupportingData
    {
        public long Ukprn { get; set; }

        public string CollectionCode { get; set; }

        public string UserEmailAddress { get; set; }

        public DateTime LastUpdatedDateTimeUtc { get; set; }
    }
}

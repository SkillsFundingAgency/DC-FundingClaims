using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.FundingClaims.Signing.Models
{
    public class FundingClaimSigningDto
    {
        public FundingClaimSigningDto(string fundingClaimId)
        {
            FundingClaimId = fundingClaimId;
        }

        public string Ukprn { get; set; }

        public bool IsSigned { get; set; }

        public int Version { get; set; }

        public string FundingClaimId { get; }

        public Guid SyndicationFeedId { get; set; }

        public DateTime UpdatedDateTimeUtc { get; set; }

        public string Period { get; set; }

        public int PageNumber { get; set; }
    }
}

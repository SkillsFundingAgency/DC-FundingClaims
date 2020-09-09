using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.FundingClaims.Model
{
    public class FundingClaimsSubmission
    {
        public string SubmissionId { get; set; }

        public DateTime SubmittedDateTime { get; set; }

        public long Ukprn { get; set; }

        public bool IsSigned { get; set; }

        public string CollectionName { get; set; }

        public string CollectionDisplayName { get; set; }
    }
}

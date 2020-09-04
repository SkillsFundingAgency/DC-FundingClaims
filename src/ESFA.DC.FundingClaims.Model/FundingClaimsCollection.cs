using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.FundingClaims.Model
{
    public class FundingClaimsCollection
    {
        public int CollectionId { get; set; }

        public DateTime SubmissionOpenDateUtc { get; set; }

        public DateTime SubmissionCloseDateUtc { get; set; }

        public DateTime? SignatureCloseDateUtc { get; set; }

        public bool? RequiresSignature { get; set; }

        public int SummarisedPeriodFrom { get; set; }

        public int SummarisedPeriodTo { get; set; }

        public string SummarisedReturnPeriod { get; set; }

        public string CollectionName { get; set; }

        public int CollectionYear { get; set; }

        public string DisplayName { get; set; }

        public bool IsOpenForSubmission { get; set; }
    }
}

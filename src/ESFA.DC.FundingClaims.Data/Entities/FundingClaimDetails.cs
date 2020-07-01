using System;

namespace ESFA.DC.FundingClaims.Data.Entities
{
    public partial class FundingClaimDetails
    {
        public string DataCollectionKey { get; set; }

        public DateTime SubmissionOpenDate { get; set; }

        public DateTime SubmissionCloseDate { get; set; }

        public DateTime SignatureCloseDate { get; set; }

        public bool RequiresSignature { get; set; }
    }
}

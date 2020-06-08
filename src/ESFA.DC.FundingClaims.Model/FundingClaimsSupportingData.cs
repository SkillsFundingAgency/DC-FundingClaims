using System;

namespace ESFA.DC.FundingClaims.Model
{
    public class FundingClaimsSupportingData
    {
        public string UserEmailAddress { get; set; }

        public DateTime SubmissionCloseDateUtc { get; set; }

        public DateTime? SignatureCloseDateUtc { get; set; }

        public string TemplateId { get; set; }
    }
}

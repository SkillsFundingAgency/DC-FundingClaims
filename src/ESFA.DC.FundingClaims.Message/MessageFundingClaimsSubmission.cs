using System;

namespace ESFA.DC.FundingClaims.Message
{
    [Serializable]
    public sealed class MessageFundingClaimsSubmission
    {
        public Guid SubmissionId { get; set; }
    }
}

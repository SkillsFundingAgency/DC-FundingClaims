using System;

namespace ESFA.DC.FundingClaims.Data.Entities
{
    public partial class FundingClaimMaxContractValues
    {
        public long Id { get; set; }

        public Guid SubmissionId { get; set; }

        public string FundingStreamPeriodCode { get; set; }

        public decimal? MaximumContractValue { get; set; }
    }
}

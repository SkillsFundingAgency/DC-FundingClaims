using System;

namespace ESFA.DC.FundingClaims.Data.Entities
{
    public partial class SubmissionContractDetail
    {
        public int Id { get; set; }

        public string FundingStreamPeriodCode { get; set; }

        public decimal ContractValue { get; set; }

        public Guid SubmissionId { get; set; }

        public virtual Submission Submission { get; set; }
    }
}

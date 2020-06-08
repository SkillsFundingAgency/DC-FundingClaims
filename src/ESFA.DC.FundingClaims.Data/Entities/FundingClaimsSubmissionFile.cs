using System;

namespace ESFA.DC.FundingClaims.Data.Entities
{
    public partial class FundingClaimsSubmissionFile
    {
        public Guid SubmissionId { get; set; }

        public string Ukprn { get; set; }

        public string CollectionPeriod { get; set; }

        public string ProviderName { get; set; }

        public DateTime UpdatedOn { get; set; }

        public string UpdatedBy { get; set; }

        public bool IsHeProvider { get; set; }

        public bool Declaration { get; set; }

        public decimal? AsbMaximumContractValue { get; set; }

        public decimal? DlsMaximumContractValue { get; set; }

        public decimal? Allb24PlsMaximumContractValue { get; set; }

        public int SubmissionType { get; set; }

        public int Version { get; set; }

        public bool IsSigned { get; set; }

        public string PeriodTypeCode { get; set; }

        public string Period { get; set; }

        public string OrganisationIdentifier { get; set; }

        public decimal? ClContractValue { get; set; }

        public DateTime? SignedOn { get; set; }

        public SubmissionTypes SubmissionTypeNavigation { get; set; }
    }
}

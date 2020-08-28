using System;
using System.Collections.Generic;

namespace ESFA.DC.FundingClaims.Data.Entities
{
    public partial class Submission
    {
        public Submission()
        {
            SubmissionContractDetail = new HashSet<SubmissionContractDetail>();
            SubmissionValue = new HashSet<SubmissionValue>();
        }

        public Guid SubmissionId { get; set; }

        public long Ukprn { get; set; }

        public string CollectionName { get; set; }

        public int CollectionYear { get; set; }

        public bool? Declaration { get; set; }

        public int Version { get; set; }

        public bool IsSigned { get; set; }

        public string OrganisationIdentifier { get; set; }

        public string SubmittedBy { get; set; }

        public bool IsSubmitted { get; set; }

        public DateTime? SubmittedDateTimeUtc { get; set; }

        public DateTime? SignedOnDateTimeUtc { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDateTimeUtc { get; set; }

        public virtual ICollection<SubmissionContractDetail> SubmissionContractDetail { get; set; }

        public virtual ICollection<SubmissionValue> SubmissionValue { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace ESFA.DC.FundingClaims.Data.Entities
{
    public partial class Submission
    {
        public Submission()
        {
            ChangeLog = new HashSet<ChangeLog>();
            SubmissionContractDetail = new HashSet<SubmissionContractDetail>();
            SubmissionValue = new HashSet<SubmissionValue>();
        }

        public Guid SubmissionId { get; set; }

        public long Ukprn { get; set; }

        public int CollectionId { get; set; }

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

        public bool? CovidDeclaration { get; set; }

        public virtual CollectionDetail Collection { get; set; }

        public virtual ICollection<ChangeLog> ChangeLog { get; set; }

        public virtual ICollection<SubmissionContractDetail> SubmissionContractDetail { get; set; }

        public virtual ICollection<SubmissionValue> SubmissionValue { get; set; }
    }
}

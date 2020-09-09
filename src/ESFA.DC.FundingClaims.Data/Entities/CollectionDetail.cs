using System;
using System.Collections.Generic;

namespace ESFA.DC.FundingClaims.Data.Entities
{
    public partial class CollectionDetail
    {
        public CollectionDetail()
        {
            Submission = new HashSet<Submission>();
        }

        public int Id { get; set; }

        public int CollectionId { get; set; }

        public int CollectionYear { get; set; }

        public string CollectionName { get; set; }

        public DateTime SubmissionOpenDateUtc { get; set; }

        public DateTime SubmissionCloseDateUtc { get; set; }

        public DateTime? SignatureCloseDateUtc { get; set; }

        public bool? RequiresSignature { get; set; }

        public string CollectionCode { get; set; }

        public int SummarisedPeriodFrom { get; set; }

        public int SummarisedPeriodTo { get; set; }

        public string SummarisedReturnPeriod { get; set; }

        public DateTime HelpdeskOpenDateUtc { get; set; }

        public DateTime DateTimeUpdatedUtc { get; set; }

        public string UpdatedBy { get; set; }

        public string DisplayTitle { get; set; }

        public virtual ICollection<Submission> Submission { get; set; }
    }
}

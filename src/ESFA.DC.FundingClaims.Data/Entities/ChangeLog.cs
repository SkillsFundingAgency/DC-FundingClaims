using System;

namespace ESFA.DC.FundingClaims.Data.Entities
{
    public partial class ChangeLog
    {
        public int Id { get; set; }

        public Guid SubmissionId { get; set; }

        public string UserEmailAddress { get; set; }

        public DateTime UpdatedDateTimeUtc { get; set; }

        public virtual Submission Submission { get; set; }
    }
}

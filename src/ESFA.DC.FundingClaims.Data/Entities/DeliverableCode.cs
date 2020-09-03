using System.Collections.Generic;

namespace ESFA.DC.FundingClaims.Data.Entities
{
    public partial class DeliverableCode
    {
        public DeliverableCode()
        {
            SubmissionValue = new HashSet<SubmissionValue>();
        }

        public int Id { get; set; }

        public int DeliverableCodeId { get; set; }

        public string Description { get; set; }

        public string FundingStreamPeriodCode { get; set; }

        public virtual ICollection<SubmissionValue> SubmissionValue { get; set; }
    }
}

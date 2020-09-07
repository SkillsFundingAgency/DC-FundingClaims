using System.Collections.Generic;

namespace ESFA.DC.FundingClaims.Data.Entities
{
    public partial class FundingStreamPeriodDeliverableCode
    {
        public FundingStreamPeriodDeliverableCode()
        {
            SubmissionValue = new HashSet<SubmissionValue>();
        }

        public int Id { get; set; }

        public int DeliverableCode { get; set; }

        public string Description { get; set; }

        public string FundingStreamPeriodCode { get; set; }

        public virtual ICollection<SubmissionValue> SubmissionValue { get; set; }
    }
}

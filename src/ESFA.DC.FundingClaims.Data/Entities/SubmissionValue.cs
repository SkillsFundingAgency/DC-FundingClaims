using System;

namespace ESFA.DC.FundingClaims.Data.Entities
{
    public partial class SubmissionValue
    {
        public long Id { get; set; }

        public Guid SubmissionId { get; set; }

        public int DeliverableCodeId { get; set; }

        public decimal DeliveryToDate { get; set; }

        public decimal ForecastedDelivery { get; set; }

        public decimal ExceptionalAdjustments { get; set; }

        public decimal TotalDelivery { get; set; }

        public int StudentNumbers { get; set; }

        public string FundingStreamPeriodCode { get; set; }

        public string ContractAllocationNumber { get; set; }

        public virtual FundingStreamPeriodDeliverableCode FundingStreamPeriodDeliverableCode { get; set; }

        public virtual Submission Submission { get; set; }
    }
}

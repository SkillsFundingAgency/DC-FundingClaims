using System;

namespace ESFA.DC.FundingClaims.Data.Entities
{
    public partial class FundingClaimsSubmissionValues
    {
        public long Id { get; set; }

        public Guid SubmissionId { get; set; }

        public string CollectionPeriod { get; set; }

        public int DeliverableCode { get; set; }

        public string DeliverableDescription { get; set; }

        public decimal? DeliveryToDate { get; set; }

        public decimal? ForecastedDelivery { get; set; }

        public decimal? ExceptionalAdjustments { get; set; }

        public decimal? TotalDelivery { get; set; }

        public string FundingStreamPeriodCode { get; set; }

        public string ContractAllocationNumber { get; set; }

        public int? StudentNumbers { get; set; }
    }
}

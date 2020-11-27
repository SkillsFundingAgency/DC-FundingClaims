namespace ESFA.DC.FundingClaims.Model
{
    public class FundingClaimsDataItem
    {
        public long Id { get; set; }

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

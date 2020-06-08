namespace ESFA.DC.FundingClaims.Model
{
    public class FundingClaimsFieldRule
    {
        public string CollectionKey { get; set; }

        public string FundingStreamPeriodCode { get; set; }

        public string DeliverableCode { get; set; }

        public string MappedColumnName { get; set; }

        public bool IsVisible { get; set; } = true;

        public bool IsReadonly { get; set; }
    }
}

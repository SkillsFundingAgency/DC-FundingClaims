namespace ESFA.DC.FundingClaims.Data.Entities
{
    public partial class FundingClaimsFieldRule
    {
        public int Id { get; set; }

        public string DataCollectionKey { get; set; }

        public string FundingStreamPeriodCode { get; set; }

        public int DeliverableCode { get; set; }

        public string MappedColumnName { get; set; }

        public bool IsVisible { get; set; }

        public bool IsEditable { get; set; }
    }
}

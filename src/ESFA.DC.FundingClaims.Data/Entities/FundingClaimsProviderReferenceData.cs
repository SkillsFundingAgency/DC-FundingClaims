namespace ESFA.DC.FundingClaims.Data.Entities
{
    public partial class FundingClaimsProviderReferenceData
    {
        public int Ukprn { get; set; }

        public bool EditAccess { get; set; }

        public decimal? AebcClallocation { get; set; }
    }
}

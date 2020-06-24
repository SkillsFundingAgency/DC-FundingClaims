using ESFA.DC.FundingClaims.Signing.Models;

namespace ESFA.DC.FunidngClaims.Signing.Services.Interfaces
{
    public interface IFeedItemMappingService
    {
        FundingClaimSigningDto Map(FundingClaimsFeedItem feedItem);

    }
}

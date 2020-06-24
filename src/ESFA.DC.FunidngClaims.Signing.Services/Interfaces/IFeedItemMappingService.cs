using System;
using ESFA.DC.FundingClaims.Signing.Models;

namespace ESFA.DC.FunidngClaims.Signing.Services.Config.Interfaces
{
    public interface IFeedItemMappingService
    {
        FundingClaimDto Map(FundingClaimsFeedItem feedItem);

    }
}

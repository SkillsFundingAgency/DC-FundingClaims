using System;
using ESFA.DC.FundingClaims.Data.Entities;
using ESFA.DC.FundingClaims.Signing.Models;

namespace ESFA.DC.FunidngClaims.Signing.Services.Interfaces
{
    public interface IFeedItemMappingService
    {
        FundingClaimSigningDto Map(DateTime updatedDateTime, string syndicationFeedId, FundingClaimsFeedItem feedItem);
        SigningNotificationFeed Map(FundingClaimSigningDto dto);
    }
}

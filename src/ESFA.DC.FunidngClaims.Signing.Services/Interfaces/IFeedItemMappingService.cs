using System;
using System.ServiceModel.Syndication;
using ESFA.DC.FundingClaims.Data.Entities;
using ESFA.DC.FundingClaims.Signing.Models;

namespace ESFA.DC.FunidngClaims.Signing.Services.Interfaces
{
    public interface IFeedItemMappingService
    {
        FundingClaimSigningDto Convert(int currentPageNumber, SyndicationItem feedItem);
    }
}

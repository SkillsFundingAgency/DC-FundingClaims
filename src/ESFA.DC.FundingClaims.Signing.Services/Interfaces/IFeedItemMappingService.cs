using System.ServiceModel.Syndication;
using ESFA.DC.FundingClaims.Signing.Models;

namespace ESFA.DC.FundingClaims.Signing.Services.Interfaces
{
    public interface IFeedItemMappingService
    {
        FundingClaimSigningDto Convert(int currentPageNumber, SyndicationItem feedItem);
    }
}

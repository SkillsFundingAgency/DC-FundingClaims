using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FundingClaims.Signing.Models;

namespace ESFA.DC.FundingClaims.Signing.Services.Interfaces
{
    public interface IFeedRepository
    {
        Task<LastSigninNotificationFeed> GetLatestSyndicationDataAsync(CancellationToken cancellationToken);
        Task SaveFeedItemAsync(CancellationToken cancellationToken, FundingClaimSigningDto feedItem);
        Task UpdateSubmissionFileAsync(CancellationToken cancellationToken, List<FundingClaimSigningDto> feedItems);
    }
}

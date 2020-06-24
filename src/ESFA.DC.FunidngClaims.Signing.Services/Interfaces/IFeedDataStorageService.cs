using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FundingClaims.Signing.Models;

namespace ESFA.DC.FunidngClaims.Signing.Services.Interfaces
{
    public interface IFeedDataStorageService
    {
        Task<IEnumerable<string>> GetExistingFeedItemIdsAsync(CancellationToken cancellationToken);

        Task<IEnumerable<string>> SaveFeedItems(CancellationToken cancellationToken,  IEnumerable<FundingClaimSigningDto> feedItems);
    }
}

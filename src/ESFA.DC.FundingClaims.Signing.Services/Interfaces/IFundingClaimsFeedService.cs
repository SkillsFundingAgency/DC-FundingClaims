using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FundingClaims.Signing.Models;

namespace ESFA.DC.FundingClaims.Signing.Services.Interfaces
{
    public interface IFundingClaimsFeedService
    {
        Task<List<FundingClaimSigningDto>> GetNewDataFromFeedAsync(CancellationToken cancellationToken, LastSigninNotificationFeed lastStoredFeedRecord);
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}

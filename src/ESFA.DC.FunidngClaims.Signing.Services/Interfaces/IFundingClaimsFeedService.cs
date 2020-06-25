using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FundingClaims.Signing.Models;

namespace ESFA.DC.FunidngClaims.Signing.Services.Interfaces
{
    public interface IFundingClaimsFeedService
    {
        Task<List<FundingClaimSigningDto>> GetNewDataFromFeedAsync(string uri, IEnumerable<Guid> existingItemIds,
            CancellationToken cancellationToken);

        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}

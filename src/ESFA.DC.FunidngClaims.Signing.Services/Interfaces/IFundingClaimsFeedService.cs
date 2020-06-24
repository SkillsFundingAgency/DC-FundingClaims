using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FundingClaims.Signing.Models;

namespace ESFA.DC.FunidngClaims.Signing.Services.Interfaces
{
    public interface IFundingClaimsFeedService
    {
        Task<IEnumerable<FundingClaimSigningDto>> GetNewDataFromFeedAsync(string uri, IEnumerable<string> existingItemIds,
            CancellationToken cancellationToken);

        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}

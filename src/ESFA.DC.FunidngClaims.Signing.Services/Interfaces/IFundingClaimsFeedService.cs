using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FundingClaims.Signing.Models;

namespace ESFA.DC.FunidngClaims.Signing.Services.Config.Interfaces
{
    public interface IFundingClaimsFeedService
    {
        Task<IEnumerable<FundingClaimDto>> GetNewDataFromFeedAsync(string uri, IEnumerable<Guid> existingItemIds,
            CancellationToken cancellationToken);

        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}

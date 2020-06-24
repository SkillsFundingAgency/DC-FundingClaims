using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FundingClaims.Signing.Models;

namespace ESFA.DC.FunidngClaims.Signing.Services.Config.Interfaces
{
    public interface IFeedDataStorageService
    {
        Task<IEnumerable<Guid>> GetExistingFeedItemIds(CancellationToken cancellationToken);

        Task<IEnumerable<Guid>> SaveFeedItems(CancellationToken cancellationToken,  IEnumerable<FundingClaimDto> feedItems);


    }
}

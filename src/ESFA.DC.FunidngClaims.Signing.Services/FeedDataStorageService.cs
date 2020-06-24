using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FundingClaims.Signing.Models;
using ESFA.DC.FunidngClaims.Signing.Services.Config.Interfaces;

namespace ESFA.DC.FunidngClaims.Signing.Services
{
    public class FeedDataStorageService : IFeedDataStorageService
    {
        public Task<IEnumerable<Guid>> GetExistingFeedItemIds(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Guid>> SaveFeedItems(CancellationToken cancellationToken, IEnumerable<FundingClaimDto> feedItems)
        {
            throw new NotImplementedException();
        }
    }
}

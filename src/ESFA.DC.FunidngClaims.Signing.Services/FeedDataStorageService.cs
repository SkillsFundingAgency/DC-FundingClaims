using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FundingClaims.Data;
using ESFA.DC.FundingClaims.Signing.Models;
using ESFA.DC.FunidngClaims.Signing.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.FunidngClaims.Signing.Services
{
    public class FeedDataStorageService : IFeedDataStorageService
    {
        private readonly Func<IFundingClaimsDataContext> _fundingclaimsDataContextFactory;

        public FeedDataStorageService(Func<IFundingClaimsDataContext> fundingclaimsDataContextFactory)
        {
            _fundingclaimsDataContextFactory = fundingclaimsDataContextFactory;
        }

        public async Task<IEnumerable<string>> GetExistingFeedItemIdsAsync(CancellationToken cancellationToken)
        {
            List<string> items = new List<string>();
            using (var context = _fundingclaimsDataContextFactory())
            {
                items = await context.SigningNotificationFeed.Select(x => x.FundingClaimId)
                    .Distinct()
                    .ToListAsync(cancellationToken);
            }

            return items;

        }

        public Task<IEnumerable<string>> SaveFeedItems(CancellationToken cancellationToken, IEnumerable<FundingClaimSigningDto> feedItems)
        {
            throw new NotImplementedException();
        }
    }
}

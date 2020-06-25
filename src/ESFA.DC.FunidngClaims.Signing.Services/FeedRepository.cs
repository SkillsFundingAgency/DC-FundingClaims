using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FundingClaims.Data;
using ESFA.DC.FundingClaims.Data.Entities;
using ESFA.DC.FundingClaims.Signing.Models;
using ESFA.DC.FunidngClaims.Signing.Services.Interfaces;
using ESFA.DC.Logging.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.FunidngClaims.Signing.Services
{
    public class FeedRepository : IFeedRepository
    {
        private readonly Func<IFundingClaimsDataContext> _fundingclaimsDataContextFactory;
        private readonly IFeedItemMappingService _feedItemMappingService;
        private readonly ILogger _logger;

        public FeedRepository(Func<IFundingClaimsDataContext> fundingclaimsDataContextFactory, IFeedItemMappingService feedItemMappingService, ILogger logger)
        {
            _fundingclaimsDataContextFactory = fundingclaimsDataContextFactory;
            _feedItemMappingService = feedItemMappingService;
            _logger = logger;
        }

        public async Task<LastSigninNotificationFeed> GetLatestSyndicationDataAsync(CancellationToken cancellationToken)
        {
            LastSigninNotificationFeed data;
            using (var context = _fundingclaimsDataContextFactory())
            {
                data = await context.SigningNotificationFeed
                    .Select(x => new LastSigninNotificationFeed()
                    {
                        LastDateTime = x.FeedDateTimeUtc,
                        LatestFeedUri = x.LatestFeedUri,
                        LatestSyndicationItemId = x.SyndicationFeedId
                    })
                    .SingleOrDefaultAsync(cancellationToken);
            }

            return data;
        }

        public async Task SaveFeedItemsAsync(CancellationToken cancellationToken, List<FundingClaimSigningDto> feedItems)
        {
            try
            {
                if (!feedItems.Any())
                {
                    _logger.LogInfo("No new items to save for funding claim feed items");
                    return;
                }

                using (var context = _fundingclaimsDataContextFactory())
                {
                    context.SigningNotificationFeed.Add( _feedItemMappingService.Map(feedItems.Last()));
                    await context.SaveChangesAsync(cancellationToken);
                }

                _logger.LogInfo($"Funding claims Signing updates - Added log and updated {feedItems.Count}");
            }
            catch (Exception e)
            {
                _logger.LogError("Error occured while saving Feed items", e);
                throw;
            }
        }

        public async Task UpdateSubmissionFileAsync(CancellationToken cancellationToken, List<FundingClaimSigningDto> feedItems)
        {
            try
            {
                if (!feedItems.Any())
                {
                    _logger.LogInfo("Nothing to update as no new items for funding claim feed items");
                    return;
                }

                using (var context = _fundingclaimsDataContextFactory())
                {

                    var data = context.FundingClaimsSubmissionFile.Where(x => x.Period == )

                    context.SigningNotificationFeed.AddRange(feedItems.Select(x => _feedItemMappingService.Map(x)));
                    await context.SaveChangesAsync(cancellationToken);
                }

                _logger.LogInfo($"Funding claims Signing updates - Added log and updated {feedItems.Count}");
            }
            catch (Exception e)
            {
                _logger.LogError("Error occured while saving Feed items", e);
                throw;
            }

        }
    }
}

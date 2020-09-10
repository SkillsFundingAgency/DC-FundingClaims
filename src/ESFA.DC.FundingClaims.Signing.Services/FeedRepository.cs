using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.FundingClaims.Data;
using ESFA.DC.FundingClaims.Data.Entities;
using ESFA.DC.FundingClaims.Signing.Models;
using ESFA.DC.FundingClaims.Signing.Services.Interfaces;
using ESFA.DC.Logging.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.FundingClaims.Signing.Services
{
    public class FeedRepository : IFeedRepository
    {
        private readonly Func<IFundingClaimsDataContext> _fundingclaimsDataContextFactory;
        private readonly IFeedItemMappingService _feedItemMappingService;
        private readonly ILogger _logger;
        private readonly IDateTimeProvider _dateTimeProvider;

        public FeedRepository(
            Func<IFundingClaimsDataContext> fundingclaimsDataContextFactory,
            IFeedItemMappingService feedItemMappingService,
            ILogger logger,
            IDateTimeProvider dateTimeProvider)
        {
            _fundingclaimsDataContextFactory = fundingclaimsDataContextFactory;
            _feedItemMappingService = feedItemMappingService;
            _logger = logger;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<LastSigninNotificationFeed> GetLatestSyndicationDataAsync(CancellationToken cancellationToken)
        {
            LastSigninNotificationFeed data;
            using (var context = _fundingclaimsDataContextFactory())
            {
                data = await context.SigningNotificationFeed
                    .Select(x => new LastSigninNotificationFeed()
                    {
                        FeedDateTimeUtc = x.FeedDateTimeUtc,
                        PageNumber = x.PageNumber,
                        SyndicationItemId = x.SyndicationFeedId,
                        DateTimeUpdatedUtc = x.DateTimeUpdatedUtc

                    })
                    .SingleOrDefaultAsync(cancellationToken);
            }

            return data;
        }

        public async Task SaveFeedItemAsync(CancellationToken cancellationToken, FundingClaimSigningDto feedItem)
        {
            try
            {
                if (feedItem == null)
                {
                    _logger.LogInfo("No new items to save for funding claim feed items");
                    return;
                }

                using (var context = _fundingclaimsDataContextFactory())
                {
                    var entry = await context.SigningNotificationFeed.SingleOrDefaultAsync(cancellationToken);
                    if (entry == null)
                    {
                        entry = new SigningNotificationFeed();
                        context.SigningNotificationFeed.Add(entry);
                    }

                    entry.FeedDateTimeUtc = feedItem.FeedDateTimeUtc;
                    entry.PageNumber = feedItem.PageNumber;
                    entry.SyndicationFeedId = feedItem.SyndicationFeedId;
                    entry.DateTimeUpdatedUtc = _dateTimeProvider.GetNowUtc();

                    await context.SaveChangesAsync(cancellationToken);
                }

                _logger.LogInfo($"Funding claims Signing updates - Added {feedItem}");
            }
            catch (Exception e)
            {
                _logger.LogError("Error occured while saving latest feed item", e);
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

                var collectionNames = feedItems.Select(x => x.CollectionName).Distinct();
                var ukpns = feedItems.Select(x => x.Ukprn).Distinct();

                using (var context = _fundingclaimsDataContextFactory())
                {

                    var submissionFiles = context.Submission.Where(x => 
                                                collectionNames.Contains(x.Collection.CollectionName) &&
                                                ukpns.Contains(x.Ukprn));

                    foreach (var feedItem in feedItems)
                    {
                        var submissionFile = await submissionFiles.SingleOrDefaultAsync(x =>
                            x.Ukprn == feedItem.Ukprn &&
                            x.Version == feedItem.Version &&
                            x.Collection.CollectionName == feedItem.CollectionName, cancellationToken);

                        if (submissionFile == null)
                        {
                            _logger.LogError(
                                $"Submission Not found - Signing notification received for ukprn : {feedItem.Ukprn}, collection : {feedItem.CollectionName} and version : {feedItem.Version}");
                            continue;
                        }

                        if (submissionFile.SignedOnDateTimeUtc.HasValue)
                        {
                            _logger.LogInfo(
                                $"Submission found but already set to signed - Signing notification received for ukprn : {feedItem.Ukprn}, collection : {feedItem.CollectionName} and version : {feedItem.Version}");
                            continue;
                        }

                        submissionFile.IsSigned = feedItem.IsSigned;
                        submissionFile.SignedOnDateTimeUtc = feedItem.FeedDateTimeUtc;

                        await context.SaveChangesAsync(cancellationToken);
                    }
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

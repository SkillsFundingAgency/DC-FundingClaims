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
        private const string CollectionPeriodName = "FC03";

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
                        DateTimeUtc = x.UpdatedDateTimeUtc,
                        PageNumber = x.PageNumber,
                        SyndicationItemId = x.SyndicationFeedId
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
                    context.SigningNotificationFeed.Add( new SigningNotificationFeed()
                    {
                        UpdatedDateTimeUtc = feedItem.UpdatedDateTimeUtc,
                        PageNumber= feedItem.PageNumber,
                        SyndicationFeedId = feedItem.SyndicationFeedId
                    });

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

                var academicYears = feedItems.Select(x => x.Period).Distinct();
                var ukpns = feedItems.Select(x => x.Ukprn).Distinct();

                using (var context = _fundingclaimsDataContextFactory())
                {

                    var submissionFiles = context.FundingClaimsSubmissionFile.Where(x => 
                                                x.CollectionPeriod == CollectionPeriodName &&
                                                academicYears.Contains(x.Period)
                                                && ukpns.Contains(x.Ukprn));

                    foreach (var feedItem in feedItems)
                    {
                        var submissionFile = await submissionFiles.SingleOrDefaultAsync(x =>
                            x.Ukprn == feedItem.Ukprn &&
                            x.Version == feedItem.Version &&
                            x.Period == feedItem.Period, cancellationToken);

                        if (submissionFile == null)
                        {
                            _logger.LogError(
                                $"Submission Not found - Signing notification received for ukprn : {feedItem.Ukprn}, period : {feedItem.Period} and version : {feedItem.Version}");
                            continue;
                        }

                        if (submissionFile.SignedOn.HasValue)
                        {
                            _logger.LogInfo(
                                $"Submission found but already set to signed - Signing notification received for ukprn : {feedItem.Ukprn}, period : {feedItem.Period} and version : {feedItem.Version}");
                            continue;
                        }

                        submissionFile.IsSigned = feedItem.IsSigned;
                        submissionFile.SignedOn = feedItem.UpdatedDateTimeUtc;

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

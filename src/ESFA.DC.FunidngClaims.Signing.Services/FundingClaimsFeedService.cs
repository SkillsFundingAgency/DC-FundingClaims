using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FundingClaims.AtomFeed.Services;
using ESFA.DC.FundingClaims.AtomFeed.Services.Config;
using ESFA.DC.FundingClaims.AtomFeed.Services.Interfaces;
using ESFA.DC.FundingClaims.Signing.Models;
using ESFA.DC.FunidngClaims.Signing.Services.Interfaces;
using ESFA.DC.Logging.Interfaces;
using Polly;
using Polly.Retry;

namespace ESFA.DC.FunidngClaims.Signing.Services
{
    public class FundingClaimsFeedService : IFundingClaimsFeedService
    {
        private readonly ISyndicationFeedService _syndicationFeedService;
        private readonly ISyndicationFeedParserService<FundingClaimsFeedItem> _syndicationFeedParserService;
        private readonly IFeedItemMappingService _feedItemMappingService;
        private readonly IFeedRepository _feedRepository;
        private readonly AtomFeedSettings _atomFeedSettings;
        private readonly ILogger _logger;

        private readonly AsyncRetryPolicy _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(5));

        public FundingClaimsFeedService(
            ISyndicationFeedService syndicationFeedService,
            ISyndicationFeedParserService<FundingClaimsFeedItem> syndicationFeedParserService,
            IFeedItemMappingService feedItemMappingService,
            IFeedRepository feedRepository,
            AtomFeedSettings atomFeedSettings,
            ILogger logger)
        {
            _syndicationFeedService = syndicationFeedService;
            _syndicationFeedParserService = syndicationFeedParserService;
            _feedItemMappingService = feedItemMappingService;
            _feedRepository = feedRepository;
            _atomFeedSettings = atomFeedSettings;
            _logger = logger;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                var latestFeedDetails = await _feedRepository.GetLatestSyndicationDataAsync(cancellationToken);

                var syncResult = await GetNewDataFromFeedAsync(cancellationToken, latestFeedDetails);

                var latestPageNumber = syncResult.OrderByDescending(x => x.PageNumber).FirstOrDefault()?.PageNumber;
                var lastItem = syncResult.LastOrDefault(x => x.PageNumber == latestPageNumber);

                await _feedRepository.SaveFeedItemAsync(cancellationToken, lastItem);

                await _feedRepository.UpdateSubmissionFileAsync(cancellationToken, syncResult);
            }
            catch (Exception exception)
            {
                _logger.LogError("Funding claims signing notifications Feed retrieval Failed", exception);
                throw;
            }
        }

        public async Task<List<FundingClaimSigningDto>> GetNewDataFromFeedAsync(CancellationToken cancellationToken, LastSigninNotificationFeed lastStoredFeedRecord)
        {

            string previousPageUri = _atomFeedSettings.DefaultFeedUri;
            var feedItemsCache = new Dictionary<string, FundingClaimSigningDto>();
            List<SyndicationItem> newItems = null;
            int currentPageNumber;

            do
            {
                _logger.LogDebug($"Funding claims signing feed - Load Syndication Feed from : {previousPageUri}");

                var feed = await _retryPolicy.ExecuteAsync(async () => await _syndicationFeedService.LoadSyndicationFeedFromUriAsync(previousPageUri, cancellationToken));

                currentPageNumber = _syndicationFeedParserService.CurrentPageNumber(feed);

                if (feed?.Items == null)
                {
                    break;
                }

                if (currentPageNumber < lastStoredFeedRecord?.PageNumber)
                {
                    break;
                }

                var lastFeedItem = feed.Items.Last();

                if (lastStoredFeedRecord != null && currentPageNumber == lastStoredFeedRecord.PageNumber)
                {
                    if (lastFeedItem.Id.SyndicationId() == lastStoredFeedRecord.SyndicationItemId)
                    {
                        break;
                    }
                    newItems = feed.Items.Where(x => x.Id.SyndicationId() != lastStoredFeedRecord.SyndicationItemId).ToList();
                }
                else
                {
                    newItems = feed.Items.ToList();
                }

                _logger.LogInfo($"New feed items found for the page : {previousPageUri}, count : {feed.Items.Count()}");

                foreach (var feedItem in newItems)
                {
                    var feedItemDto = _feedItemMappingService.Convert(currentPageNumber, feedItem);
                    feedItemsCache.Add(feedItem.Id, feedItemDto);
                }

                previousPageUri = _syndicationFeedParserService.PreviousArchiveLink(feed);
            }
            while (ContinueToNextPage(previousPageUri, newItems.Any()));

            return feedItemsCache.Values.ToList();
        }

        public bool ContinueToNextPage(string previousPageUri, bool hasAnyNewFeedItems)
        {
            return previousPageUri != null && hasAnyNewFeedItems;
        }
    }
}

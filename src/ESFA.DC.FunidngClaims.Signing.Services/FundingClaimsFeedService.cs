using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

                var newItems = await GetNewDataFromFeedAsync(latestFeedDetails, cancellationToken);

                 await _feedRepository.SaveFeedItemsAsync(cancellationToken, newItems);
            }
            catch (Exception exception)
            {
                _logger.LogError("Funding claims signing notifications Feed retrieval Failed", exception);
                throw;
            }
        }

        public async Task<List<FundingClaimSigningDto>> GetNewDataFromFeedAsync(LastSigninNotificationFeed latestFeed, CancellationToken cancellationToken)
        {
            
            string previousPageUri = latestFeed?.LatestFeedUri ?? _atomFeedSettings.DefaultFeedUri;

            var feedItemsCache = new Dictionary<string, FundingClaimSigningDto>();
            bool existingFeedItemFound;

            do
            {
                _logger.LogDebug($"Funding claims signing feed - Load Syndication Feed from : {previousPageUri}");

                var feed = await _retryPolicy.ExecuteAsync(async () => await _syndicationFeedService.LoadSyndicationFeedFromUriAsync(previousPageUri, cancellationToken));

                existingFeedItemFound = feed.Items.Any(x => x.Id == latestFeed?.LatestSyndicationItemId);

                if (existingFeedItemFound)
                {
                    _logger.LogInfo($"We already have data for {feed.Id} so skipping this page : {previousPageUri}");
                    continue;
                }

                foreach (var feedItem in feed.Items)
                {
                    var feedItemDetails = _syndicationFeedParserService.RetrieveDataFromSyndicationItem(feedItem);

                    var feedItemDto = _feedItemMappingService.Map(feedItem.LastUpdatedTime.DateTime, feedItem.Id,
                        feedItemDetails);

                    feedItemsCache.Add(feedItem.Id, feedItemDto);
                }

                previousPageUri = _syndicationFeedParserService.PreviousArchiveLink(feed);
            }
            while (ContinueToNextPage(previousPageUri, existingFeedItemFound));

            return feedItemsCache.Values.ToList();
        }

        public bool ContinueToNextPage(string previousPageUri, bool hasAnyNewFeedItems)
        {
            return previousPageUri != null && hasAnyNewFeedItems;
        }
    }
}

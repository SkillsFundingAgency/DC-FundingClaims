using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FundingClaims.AtomFeed.Services.Config;
using ESFA.DC.FundingClaims.AtomFeed.Services.Interfaces;
using ESFA.DC.FundingClaims.Signing.Models;
using ESFA.DC.FunidngClaims.Signing.Services.Interfaces;
using ESFA.DC.Logging.Interfaces;
using Polly;
using Polly.Retry;

namespace ESFA.DC.ReferenceData.FCS.Service
{
    public class FundingClaimsFeedService : IFundingClaimsFeedService
    {
        private readonly ISyndicationFeedService _syndicationFeedService;
        private readonly ISyndicationFeedParserService<FundingClaimsFeedItem> _syndicationFeedParserService;
        private readonly IFeedItemMappingService _feedItemMappingService;
        private readonly IFeedDataStorageService _feedDataStorageService;
        private readonly AtomFeedSettings _atomFeedSettings;
        private readonly ILogger _logger;

        private readonly AsyncRetryPolicy _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(5));

        public FundingClaimsFeedService(
            ISyndicationFeedService syndicationFeedService,
            ISyndicationFeedParserService<FundingClaimsFeedItem> syndicationFeedParserService,
            IFeedItemMappingService feedItemMappingService,
            IFeedDataStorageService feedDataStorageService,
            AtomFeedSettings atomFeedSettings,
            ILogger logger)
        {
            _syndicationFeedService = syndicationFeedService;
            _syndicationFeedParserService = syndicationFeedParserService;
            _feedItemMappingService = feedItemMappingService;
            _feedDataStorageService = feedDataStorageService;
            _atomFeedSettings = atomFeedSettings;
            _logger = logger;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                var existingItemIds = await _feedDataStorageService.GetExistingFeedItemIdsAsync(cancellationToken);

                var newItems = await GetNewDataFromFeedAsync(_atomFeedSettings.FeedUri, existingItemIds, cancellationToken);

                await _feedDataStorageService.SaveFeedItems(cancellationToken, newItems);
            }
            catch (Exception exception)
            {
                _logger.LogError("Funding claims signing notifications Feed retrieval Failed", exception);
                throw;
            }
        }

        public async Task<IEnumerable<FundingClaimSigningDto>> GetNewDataFromFeedAsync(string uri, IEnumerable<string> existingItemIds, CancellationToken cancellationToken)
        {
            
            string previousPageUri = uri;
            var contractorCache = new Dictionary<string, FundingClaimSigningDto>();
            IEnumerable<string> newCurrentPageItemIds;

            var existingSyndicationItemIdsHashSet = new HashSet<string>(existingItemIds);


            do
            {

                _logger.LogDebug($"Funding claims signing feed - Load Syndication Feed from : {previousPageUri}");

                var feed = await _retryPolicy.ExecuteAsync(async () => await _syndicationFeedService.LoadSyndicationFeedFromUriAsync(previousPageUri, cancellationToken));

                _syndicationFeedParserService.RetrieveDataFromSyndicationItem(feed);

                var feedItems = feed
                    .Items
                    .Reverse()
                    .Select(_syndicationFeedParserService.RetrieveDataFromSyndicationItem)
                    .Where(m => !existingSyndicationItemIdsHashSet.Contains(m.model.FundingClaimId))
                    .Select(m => _feedItemMappingService.Map(m.model))
                    .ToList();

                newCurrentPageItemIds = feedItems.Select(c => c.FeedItemId);

                foreach (var feedItem in feedItems)
                {
                    if (!contractorCache.ContainsKey(feedItem.FeedItemId))
                    {
                        contractorCache.Add(feedItem.FeedItemId, feedItem);
                    }
                }

                previousPageUri = _syndicationFeedParserService.PreviousArchiveLink(feed);
            }
            while (ContinueToNextPage(previousPageUri, newCurrentPageItemIds));

            return contractorCache.Values;
        }

        public bool ContinueToNextPage(string previousPageUri, IEnumerable<string> newCurrentPageItemIds)
        {
            return previousPageUri != null && newCurrentPageItemIds.Any();
        }
    }
}

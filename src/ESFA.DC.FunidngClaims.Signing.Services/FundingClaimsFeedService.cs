using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.ReferenceData.FCS.Model;
using ESFA.DC.ReferenceData.FCS.Service.Interface;


namespace ESFA.DC.ReferenceData.FCS.Service
{
    public class FundingClaimsFeedService : IFcsFeedService
    {
        private readonly ISyndicationFeedService _syndicationFeedService;
        private readonly IFcsSyndicationFeedParserService _fcsSyndicationFeedParserService;
        private readonly IContractMappingService _contractMappingService;
        private readonly ILogger _logger;

        private readonly RetryPolicy _retryPolicy =
            Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, a => TimeSpan.FromSeconds(3));

        public FundingClaimsFeedService(
            ISyndicationFeedService syndicationFeedService,
            IFcsSyndicationFeedParserService fcsSyndicationFeedParserService,
            IContractMappingService contractMappingService,
            ILogger logger)
        {
            _syndicationFeedService = syndicationFeedService;
            _fcsSyndicationFeedParserService = fcsSyndicationFeedParserService;
            _contractMappingService = contractMappingService;
            _logger = logger;
        }

        public async Task<IEnumerable<Contractor>> GetNewDataFromFeedAsync(string uri, IEnumerable<Guid> existingSyndicationItemIds, CancellationToken cancellationToken)
        {
            string previousPageUri = uri;
            var existingSyndicationItemIdsHashSet = new HashSet<Guid>(existingSyndicationItemIds.Distinct());

            var contractorCache = new Dictionary<string, Contractor>();

            IEnumerable<Guid> newCurrentPageSyndicationItemIds;

            do
            {
                _logger.LogVerbose($"FCS Contracts Reference Data - Load Syndication Feed from : {previousPageUri}");

                var feed = await _retryPolicy.ExecuteAsync(async () => await _syndicationFeedService.LoadSyndicationFeedFromUriAsync(previousPageUri, cancellationToken));

                var contractors = feed
                    .Items
                    .Reverse()
                    .Select(_fcsSyndicationFeedParserService.RetrieveContractFromSyndicationItem)
                    .Where(m => !existingSyndicationItemIdsHashSet.Contains(m.syndicationItemId))
                    .Select(m => _contractMappingService.Map(m.syndicationItemId, m.contract))
                    .ToList();

                newCurrentPageSyndicationItemIds = contractors.Where(x => x.SyndicationItemId != null).Select(c => c.SyndicationItemId.GetValueOrDefault(Guid.Empty));

                foreach (var contractor in contractors)
                {
                    var contractNumber = contractor.Contracts.First().ContractNumber;

                    if (!contractorCache.ContainsKey(contractNumber))
                    {
                        contractorCache.Add(contractNumber, contractor);
                    }
                }

                previousPageUri = _fcsSyndicationFeedParserService.PreviousArchiveLink(feed);
            }
            while (ContinueToNextPage(previousPageUri, newCurrentPageSyndicationItemIds));

            return contractorCache.Values;
        }

        public bool ContinueToNextPage(string previousPageUri, IEnumerable<Guid> newCurrentPageSyndicationItemIds)
        {
            return previousPageUri != null && newCurrentPageSyndicationItemIds.Any();
        }
    }
}

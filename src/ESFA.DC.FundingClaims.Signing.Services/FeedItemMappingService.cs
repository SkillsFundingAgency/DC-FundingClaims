using System;
using System.ServiceModel.Syndication;
using ESFA.DC.FundingClaims.AtomFeed.Services;
using ESFA.DC.FundingClaims.AtomFeed.Services.Interfaces;
using ESFA.DC.FundingClaims.Signing.Models;
using ESFA.DC.FundingClaims.Signing.Services.Interfaces;

namespace ESFA.DC.FundingClaims.Signing.Services
{
    public class FeedItemMappingService : IFeedItemMappingService
    {
        private readonly ISyndicationFeedParserService<FundingClaimsFeedItem> _syndicationFeedParserService;

        public FeedItemMappingService(ISyndicationFeedParserService<FundingClaimsFeedItem> syndicationFeedParserService)
        {
            _syndicationFeedParserService = syndicationFeedParserService;
        }

        public FundingClaimSigningDto Convert(int currentPageNumber, SyndicationItem feedItem)
        {

            var feedItemDetails = _syndicationFeedParserService.RetrieveDataFromSyndicationItem(feedItem);

            if (string.IsNullOrEmpty(feedItemDetails.FundingClaimId))
            {
                throw new ArgumentNullException("Funding claim id missing");
            }

            var idParts = feedItemDetails.FundingClaimId.Split('_');
            if (idParts.Length != 3)
            {
                throw new Exception($"invalid funding claim id : {feedItemDetails.FundingClaimId}");
            }

            int.TryParse(idParts[2], out var version);

            var collectionNameParts = idParts[0].Split('-');
            
            var dto = new FundingClaimSigningDto(feedItemDetails.FundingClaimId)
            {
                IsSigned = feedItemDetails.HasBeenSigned,
                SyndicationFeedId = feedItem.Id.SyndicationId(),
                FeedDateTimeUtc = feedItem.LastUpdatedTime.UtcDateTime,
                Ukprn = idParts[1],
                Version = version,
                Period = collectionNameParts[0],
                PageNumber = currentPageNumber
            };

            return dto;
        }
    }
}

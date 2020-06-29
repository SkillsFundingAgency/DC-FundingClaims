using System;
using System.Runtime.CompilerServices;
using System.ServiceModel.Syndication;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.FundingClaims.AtomFeed.Services;
using ESFA.DC.FundingClaims.AtomFeed.Services.Interfaces;
using ESFA.DC.FundingClaims.Data.Entities;
using ESFA.DC.FundingClaims.Signing.Models;
using ESFA.DC.FunidngClaims.Signing.Services.Interfaces;

namespace ESFA.DC.FunidngClaims.Signing.Services
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

            var pieces = feedItemDetails.FundingClaimId.Split('_');
            if (pieces.Length != 3)
            {
                throw new Exception($"invalid funding claim id : {feedItemDetails.FundingClaimId}");
            }

            int.TryParse(pieces[2], out var version);

            var collectionNameParts = pieces[0].Split('-');
            
            var dto = new FundingClaimSigningDto(feedItemDetails.FundingClaimId)
            {
                IsSigned = feedItemDetails.HasBeenSigned,
                SyndicationFeedId = feedItem.Id.SyndicationId(),
                FeedDateTimeUtc = feedItem.LastUpdatedTime.UtcDateTime,
                Ukprn = pieces[1],
                Version = version,
                Period = collectionNameParts[0],
                PageNumber = currentPageNumber
            };

            return dto;
        }
    }
}

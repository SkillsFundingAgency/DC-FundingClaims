using System;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.FundingClaims.Data.Entities;
using ESFA.DC.FundingClaims.Signing.Models;
using ESFA.DC.FunidngClaims.Signing.Services.Interfaces;

namespace ESFA.DC.FunidngClaims.Signing.Services
{
    public class FeedItemMappingService : IFeedItemMappingService
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public FeedItemMappingService(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public FundingClaimSigningDto Map(DateTime updatedDateTime, string syndicationFeedId, FundingClaimsFeedItem feedItem)
        {
            if (string.IsNullOrEmpty(feedItem.FundingClaimId))
            {
                throw new ArgumentNullException("Funding claim id missing");
            }

            var pieces = feedItem.FundingClaimId.Split('_');
            if (pieces.Length != 3)
            {
                throw new Exception($"invalid funding claim id : {feedItem.FundingClaimId}");
            }

            var dto = new FundingClaimSigningDto(feedItem.FundingClaimId)
            {
                IsSigned = feedItem.HasBeenSigned,
                SyndicationFeedId = syndicationFeedId,
                UpdatedDateTimeUtc = updatedDateTime,
            };

            return dto;
        }

        public SigningNotificationFeed Map(FundingClaimSigningDto dto)
        {
            return new SigningNotificationFeed()
            {
               FeedDateTimeUtc = dto.UpdatedDateTimeUtc,
               //LatestFeedUri = dto
               SyndicationFeedId = dto.SyndicationFeedId,
            };
        }
    }
}

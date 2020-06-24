using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.FundingClaims.Signing.Models;
using ESFA.DC.FunidngClaims.Signing.Services.Config.Interfaces;

namespace ESFA.DC.ReferenceData.FCS.Service
{
    public class FeedItemMappingService : IFeedItemMappingService
    {
        public FundingClaimDto Map(FundingClaimsFeedItem feedItem)
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

            var dto = new FundingClaimDto(feedItem.FundingClaimId)
            {
                IsSigned = feedItem.HasBeenSigned,
            };

            return dto;
        }
    }
}

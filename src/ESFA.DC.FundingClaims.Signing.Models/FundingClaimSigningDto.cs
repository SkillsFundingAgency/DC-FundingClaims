﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.FundingClaims.Signing.Models
{
    public class FundingClaimSigningDto
    {
        public FundingClaimSigningDto(string fundingClaimId)
        {
            FundingClaimId = fundingClaimId;

            var pieces = fundingClaimId.Split('_');

            long.TryParse(pieces[1], out var ukprn);
            int.TryParse(pieces[2], out var version);

            Ukprn = ukprn;
            CollectionPeriod = pieces[0].Split('-')[1];
            Version = version;

            int.TryParse(pieces[0].Split('-')[0], out var year);
            Year = year;
        }
        public long Ukprn { get; }

        public bool IsSigned { get; set; }

        public string CollectionPeriod { get; }

        public int Version { get; }

        public string FundingClaimId { get; }

        public string SyndicationFeedId { get; set; }

        public DateTime UpdatedDateTimeUtc { get; set; }

        public int Year { get; set; }
    }
}

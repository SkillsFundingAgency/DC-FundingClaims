using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.FundingClaims.Model
{
    public class FundingClaimsData
    {
        public long Ukprn { get; set; }

        public string CollectionName { get; set; }

        public string EmailAddress { get; set; }

        public int CollectionYear { get; set; }

        public string UserName { get; set; }

        public bool CovidDeclaration { get; set; }

        public List<FundingClaimsDataItem> FundingClaimsDataItems { get; set; } = new List<FundingClaimsDataItem>();
    }
}

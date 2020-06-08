using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.FundingClaims.Model
{
    public class FundingClaimsData
    {
        public long Ukprn { get; set; }

        public string CollectionCode { get; set; }

        public string EmailAddress { get; set; }

        public int AcademicYear { get; set; }

        public List<FundingClaimsDataItem> FundingClaimsDataItems { get; set; }
    }
}

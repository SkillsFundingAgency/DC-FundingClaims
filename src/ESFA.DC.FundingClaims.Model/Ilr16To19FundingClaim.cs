using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.FundingClaims.Model
{
    public class Ilr16To19FundingClaim
    {
        public string FundLine { get; set; }

        public string RateBand { get; set; }

        public int? StudentNumbers { get; set; }

        public decimal? DeliveryToDate { get; set; }

        public int RateBandSortOrder { get; set; }

        public int FundLineSortOrder { get; set; }
    }
}

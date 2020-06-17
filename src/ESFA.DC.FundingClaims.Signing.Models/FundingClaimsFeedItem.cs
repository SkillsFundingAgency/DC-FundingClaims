using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.FundingClaims.Signing.Models
{
    public class FundingClaimsFeedItem
    {
        public string FundingClaimId { get; set; } //Eg: 1516-Final_10000981_1

        public bool HasBeenSigned { get; set; }
    }
}

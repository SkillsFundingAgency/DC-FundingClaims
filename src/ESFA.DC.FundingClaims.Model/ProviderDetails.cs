using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.FundingClaims.Model
{
    public class ProviderDetails
    {
        public string Name { get; set; }

        public bool IsHesaProvider { get; set; }

        public long Ukprn { get; set; }
    }
}

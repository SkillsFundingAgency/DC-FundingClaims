using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.FundingClaims.Model
{
    public class ContractAllocation
    {
        public string FundingStreamPeriodCode { get; set; }

        public string ContractAllocationNumber { get; set; }

        public decimal MaximumContractValue { get; set; }

        public string Period { get; set; }

        public string PeriodTypeCode { get; set; }

        public string OrganisationIdentifier { get; set; }
    }
}

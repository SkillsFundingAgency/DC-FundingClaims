using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.FundingClaim.Console.Interfaces
{
    public interface IFundingClaimsWebJob<MessageFundingClaimsSubmission>
    {
        void Subscribe();
    }
}

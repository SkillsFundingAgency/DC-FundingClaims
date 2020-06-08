using System;
using System.Collections.Generic;
using System.Text;
using ESFA.DC.FundingClaims.Model.Interfaces;

namespace ESFA.DC.FundingClaims.Model
{
    public class FundingStreamPeriodCodes1819 : IFundingStreamPeriodCodes
    {
        public string TraineeshipNonProcured => throw new NotImplementedException();

        public string TraineeshipProcured => throw new NotImplementedException();

        string IFundingStreamPeriodCodes.AdultEducationBudgetNonProcured => "AEBC1819";

        string IFundingStreamPeriodCodes.LearnerSupportProcured => "AEB-LS1819";

        string IFundingStreamPeriodCodes.CommunityLearning => "AEBTO-CL1819";

        string IFundingStreamPeriodCodes.AdvanceLoadBursary => "ALLBC1819";

        string IFundingStreamPeriodCodes.Ilr16To19 => "1619LR1819";
    }
}

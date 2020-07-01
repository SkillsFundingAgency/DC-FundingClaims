using System;
using System.Collections.Generic;
using System.Text;
using ESFA.DC.FundingClaims.Model.Interfaces;

namespace ESFA.DC.FundingClaims.Model
{
    public class FundingStreamPeriodCodes1920 : IFundingStreamPeriodCodes
    {
        public string TraineeshipNonProcured => "AEBC-19TRN1920";

        public string TraineeshipProcured => "AEB-19TRLS1920";

        string IFundingStreamPeriodCodes.AdultEducationBudgetNonProcured => "AEBC-ASCL1920";

        string IFundingStreamPeriodCodes.LearnerSupportProcured => "AEB-ASLS1920";

        string IFundingStreamPeriodCodes.CommunityLearning => "AEB-CL1920";

        string IFundingStreamPeriodCodes.AdvanceLoadBursary => "ALLBC1920";

        string IFundingStreamPeriodCodes.Ilr16To19 => "1619LR1819";
    }
}

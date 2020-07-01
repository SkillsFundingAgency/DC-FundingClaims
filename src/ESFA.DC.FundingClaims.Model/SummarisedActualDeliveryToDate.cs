using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.FundingClaims.Model
{
    public class SummarisedActualDeliveryToDate
    {
        public string FundingStreamPeriodCode { get; set; }

        public int DeliverableCode { get; set; }

        public decimal DeliveryToDate { get; set; }
    }
}

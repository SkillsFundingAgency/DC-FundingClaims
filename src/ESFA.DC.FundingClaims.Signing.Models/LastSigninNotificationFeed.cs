using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.FundingClaims.Signing.Models
{
    public class LastSigninNotificationFeed
    {
        public string LatestFeedUri { get; set; }

        public DateTime LastDateTime { get; set; }

        public string LatestSyndicationItemId { get; set; }   
    }
}

﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.FundingClaims.Signing.Models
{
    public class LastSigninNotificationFeed
    {
        public DateTime FeedDateTimeUtc { get; set; }

        public Guid SyndicationItemId { get; set; }

        public int PageNumber { get; set; }

        public DateTime DateTimeUpdatedUtc { get; set; }
    }
}

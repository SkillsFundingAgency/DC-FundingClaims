using System;

namespace ESFA.DC.FundingClaims.Data.Entities
{
    public partial class SigningNotificationFeed
    {
        public int Id { get; set; }

        public string FundingClaimId { get; set; }

        public DateTime DateTimeRecievedUtc { get; set; }

        public bool HasBeenSigned { get; set; }
    }
}

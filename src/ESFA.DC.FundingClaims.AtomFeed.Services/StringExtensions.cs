using System;

namespace ESFA.DC.FundingClaims.AtomFeed.Services
{
    public static class StringExtensions
    {
        public static Guid SyndicationId (this string uuid)
        {
            if (string.IsNullOrEmpty(uuid))
            {
                return Guid.Empty;
            }

            Guid.TryParse(uuid.Substring(5), out var result);

            return result;
        }
    }
}

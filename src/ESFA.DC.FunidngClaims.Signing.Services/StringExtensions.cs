using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.FunidngClaims.Signing.Services
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

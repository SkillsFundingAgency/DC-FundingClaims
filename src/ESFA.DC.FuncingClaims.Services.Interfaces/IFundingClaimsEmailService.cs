using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESFA.DC.FuncingClaims.Services.Interfaces
{
    public interface IFundingClaimsEmailService
    {
        Task<IEnumerable<string>> GetUnsubmittedClaimEmailAddressses(string collectionCode, string year, DateTime startDateTimeUtc);
    }
}

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.FundingClaims.Services.Interfaces
{
    public interface IFundingClaimsEmailService
    {
        Task<ICollection<string>> GetUnsubmittedClaimEmailAddressesAsync(CancellationToken cancellationToken, string collectionName, int year, DateTime startDateTimeUtc);
    }
}

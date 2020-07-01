using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.FundingClaims.Data;
using ESFA.DC.FundingClaims.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.FundingClaims.Services
{
    public class FundingClaimsEmailService : IFundingClaimsEmailService
    {
        private readonly Func<IFundingClaimsDataContext> _fundingClaimsContextFactory;
        private readonly IDateTimeProvider _dateTimeProvider;

        public FundingClaimsEmailService(
            Func<IFundingClaimsDataContext> fundingClaimsContextFactory,
            IDateTimeProvider dateTimeProvider)
        {
            _fundingClaimsContextFactory = fundingClaimsContextFactory;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<ICollection<string>> GetUnsubmittedClaimEmailAddressesAsync(CancellationToken cancellationToken, string collectionCode, string year, DateTime startDateTimeUtc)
        {
            var yesterday = _dateTimeProvider.GetNowUtc().AddDays(-1);

            using (var fundingClaimsContext = _fundingClaimsContextFactory())
            {
                var submittedProviders = await fundingClaimsContext.FundingClaimsSubmissionFile.Where(x =>
                        x.Period == year &&
                        x.CollectionPeriod == collectionCode)
                    .Select(x => long.Parse(x.Ukprn))
                    .Distinct()
                    .ToListAsync(cancellationToken);

                var emailAddressesList = await fundingClaimsContext.FundingClaimsSupportingData
                    .Where(c => c.CollectionCode == collectionCode &&
                                c.LastUpdatedDateTimeUtc <= yesterday && c.LastUpdatedDateTimeUtc >= startDateTimeUtc
                                && !submittedProviders.Contains(c.Ukprn))
                    .Select(x => x.UserEmailAddress)
                    .ToListAsync(cancellationToken);

                return emailAddressesList;
            }
        }
    }
}
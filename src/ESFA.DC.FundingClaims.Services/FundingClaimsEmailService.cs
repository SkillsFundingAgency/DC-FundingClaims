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

        public async Task<ICollection<string>> GetUnsubmittedClaimEmailAddressesAsync(CancellationToken cancellationToken, string collectionName, int year, DateTime startDateTimeUtc)
        {
            var yesterday = _dateTimeProvider.GetNowUtc().AddDays(-1);

            using (var fundingClaimsContext = _fundingClaimsContextFactory())
            {
                var submittedProviders = await fundingClaimsContext.Submission.Where(x =>
                        x.Collection.CollectionYear == year &&
                        x.Collection.CollectionName == collectionName &&
                        x.IsSubmitted)
                    .Distinct()
                    .Select(x => x.Ukprn)
                    .ToListAsync(cancellationToken);

                var emailAddressesList = await fundingClaimsContext.ChangeLog
                    .Where(c => c.Submission.Collection.CollectionName == collectionName &&
                                c.UpdatedDateTimeUtc <= yesterday && c.UpdatedDateTimeUtc >= startDateTimeUtc
                                && !submittedProviders.Contains(c.Submission.Ukprn))
                    .Select(x => x.UserEmailAddress)
                    .ToListAsync(cancellationToken);

                return emailAddressesList;
            }
        }
    }
}
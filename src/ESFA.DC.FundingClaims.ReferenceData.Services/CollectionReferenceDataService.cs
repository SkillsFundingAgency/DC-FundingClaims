using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.FundingClaims.Data;
using ESFA.DC.FundingClaims.Data.Entities;
using ESFA.DC.FundingClaims.Model;
using ESFA.DC.FundingClaims.ReferenceData.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.FundingClaims.ReferenceData.Services
{
    public class CollectionReferenceDataService : ICollectionReferenceDataService
    {
        private readonly Func<IFundingClaimsDataContext> _fundingClaimsDataContext;
        private readonly IDateTimeProvider _dateTimeProvider;

        public CollectionReferenceDataService(Func<IFundingClaimsDataContext> fundingClaimsDataContext, IDateTimeProvider dateTimeProvider)
        {
            _fundingClaimsDataContext = fundingClaimsDataContext;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<FundingClaimsCollection> GetFundingClaimsCollectionAsync(CancellationToken cancellationToken, string collectionName)
        {
            using (var context = _fundingClaimsDataContext())
            {
                var data = await context.CollectionDetail
                    .Where(x => x.CollectionName == collectionName)
                    .FirstOrDefaultAsync(cancellationToken);

                if (data == null)
                {
                    return null;
                }

                return Convert(data);
            }
        }

        public async Task<List<FundingClaimsCollection>> GetAllFundingClaimsCollectionsAsync(CancellationToken cancellationToken)
        {
            using (var context = _fundingClaimsDataContext())
            {
                var result = await context.CollectionDetail
                    .Select(x => Convert(x))
                    .ToListAsync(cancellationToken);
                return result;
            }
        }

        public async Task<FundingClaimsCollection> GetFundingClaimsCollectionAsync(CancellationToken cancellationToken, DateTime? dateTimeUtc = null)
        {
            dateTimeUtc = dateTimeUtc ?? _dateTimeProvider.GetNowUtc();

            using (var context = _fundingClaimsDataContext())
            {
                var data = await context.CollectionDetail
                    .SingleOrDefaultAsync(x => dateTimeUtc >= x.SubmissionOpenDateUtc && dateTimeUtc <= x.SubmissionCloseDateUtc, cancellationToken);

                if (data == null)
                {
                    return null;
                }

                return Convert(data);
            }
        }

        public async Task<FundingClaimsCollection> GetLatestFundingClaimsCollectionAsync(CancellationToken cancellationToken, bool requiresSignature)
        {
            using (var context = _fundingClaimsDataContext())
            {
                var data = await context.CollectionDetail
                    .Where(x => x.RequiresSignature == requiresSignature)
                    .OrderByDescending(x => x.SubmissionCloseDateUtc)
                    .FirstOrDefaultAsync(cancellationToken);

                if (data == null)
                {
                    return null;
                }

                return Convert(data);
            }
        }


        private FundingClaimsCollection Convert(CollectionDetail data)
        {
            var nowUtcDateTime = _dateTimeProvider.GetNowUtc();

            return new FundingClaimsCollection()
            {
                CollectionId = data.CollectionId,
                CollectionYear = data.CollectionYear,
                CollectionCode = data.CollectionCode,
                RequiresSignature = data.RequiresSignature.GetValueOrDefault(),
                SignatureCloseDateUtc = data.SignatureCloseDateUtc,
                SubmissionOpenDateUtc = data.SubmissionOpenDateUtc,
                SubmissionCloseDateUtc = data.SubmissionCloseDateUtc,
                CollectionName = data.CollectionName,
                SummarisedPeriodFrom = data.SummarisedPeriodFrom,
                SummarisedPeriodTo = data.SummarisedPeriodTo,
                SummarisedReturnPeriod = data.SummarisedReturnPeriod,
                DisplayName = data.DisplayTitle,
                IsOpenForSubmission = nowUtcDateTime >= data.SubmissionOpenDateUtc && nowUtcDateTime <= data.SubmissionCloseDateUtc
            };
        }
    }
}

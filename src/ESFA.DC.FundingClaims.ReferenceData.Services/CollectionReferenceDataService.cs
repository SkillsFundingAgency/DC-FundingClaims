using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.FundingClaims.Model;
using ESFA.DC.FundingClaims.ReferenceData.Services.Interfaces;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.FundingClaims.ReferenceData.Services
{
    public class CollectionReferenceDataService : ICollectionReferenceDataService
    {
        private readonly Func<IJobQueueDataContext> _jobQueueDataContextFactory;
        private readonly IDateTimeProvider _dateTimeProvider;

        public CollectionReferenceDataService(Func<IJobQueueDataContext> jobQueueDataContextFactory, IDateTimeProvider dateTimeProvider)
        {
            _jobQueueDataContextFactory = jobQueueDataContextFactory;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<FundingClaimsCollection> GetFundingClaimsCollectionAsync(CancellationToken cancellationToken, string collectionCode)
        {
            using (var context = _jobQueueDataContextFactory())
            {
                var data = await context.FundingClaimsCollectionMetaData.Include(x => x.Collection)
                    .Where(x => x.CollectionCode == collectionCode)
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
            using (var context = _jobQueueDataContextFactory())
            {
                var result = await context.FundingClaimsCollectionMetaData.Include(x => x.Collection)
                    .Select(x => Convert(x))
                    .ToListAsync(cancellationToken);
                return result;
            }
        }

        public async Task<FundingClaimsCollection> GetFundingClaimsCollectionAsync(CancellationToken cancellationToken, DateTime? dateTimeUtc = null)
        {
            dateTimeUtc = dateTimeUtc ?? _dateTimeProvider.GetNowUtc();

            using (var context = _jobQueueDataContextFactory())
            {
                var data = await context.FundingClaimsCollectionMetaData.Include(x => x.Collection)
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
            using (var context = _jobQueueDataContextFactory())
            {
                var data = await context.FundingClaimsCollectionMetaData.Include(x => x.Collection)
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

        public async Task<string> GetEmailTemplateAsync(CancellationToken cancellationToken, int collectionId)
        {
            using (IJobQueueDataContext context = _jobQueueDataContextFactory())
            {
                var emailTemplate = await
                    context.JobEmailTemplate.SingleOrDefaultAsync(x => x.CollectionId == collectionId
                                                                       && x.Active.Value, cancellationToken);

                return emailTemplate?.TemplateOpenPeriod ?? string.Empty;
            }
        }

        private FundingClaimsCollection Convert(FundingClaimsCollectionMetaData data)
        {
            var nowUtcDateTime = _dateTimeProvider.GetNowUtc();

            return new FundingClaimsCollection()
            {
                CollectionId = data.CollectionId,
                CollectionYear = data.Collection.CollectionYear.GetValueOrDefault(),
                CollectionCode = data.CollectionCode,
                RequiresSignature = data.RequiresSignature.GetValueOrDefault(),
                SignatureCloseDateUtc = data.SignatureCloseDateUtc,
                SubmissionOpenDateUtc = data.SubmissionOpenDateUtc,
                SubmissionCloseDateUtc = data.SubmissionCloseDateUtc,
                CollectionName = data.Collection.Name,
                SummarisedPeriodFrom = data.SummarisedPeriodFrom,
                SummarisedPeriodTo = data.SummarisedPeriodTo,
                SummarisedReturnPeriod = data.SummarisedReturnPeriod,
                DisplayName = data.Collection.Description,
                IsOpenForSubmission = nowUtcDateTime >= data.SubmissionOpenDateUtc && nowUtcDateTime <= data.SubmissionCloseDateUtc
            };
        }
    }
}

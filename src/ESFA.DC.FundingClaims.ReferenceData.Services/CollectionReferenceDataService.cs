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
using ESFA.DC.Logging.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.FundingClaims.ReferenceData.Services
{
    public class CollectionReferenceDataService : ICollectionReferenceDataService
    {
        private readonly Func<IFundingClaimsDataContext> _fundingClaimsDataContext;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILogger _logger;

        public CollectionReferenceDataService(Func<IFundingClaimsDataContext> fundingClaimsDataContext, IDateTimeProvider dateTimeProvider, ILogger logger)
        {
            _fundingClaimsDataContext = fundingClaimsDataContext;
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
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

        public async Task<FundingClaimsCollection> GetLatestFundingClaimsCollectionAsync(CancellationToken cancellationToken, bool? requiresSignature = null)
        {
            using (var context = _fundingClaimsDataContext())
            {
                var query = context.CollectionDetail.AsQueryable();
                if (requiresSignature.HasValue)
                {
                    query = query.Where(x => x.RequiresSignature == requiresSignature);
                }

                var data = await query.OrderByDescending(x => x.SubmissionCloseDateUtc)
                    .FirstOrDefaultAsync(cancellationToken);

                if (data == null)
                {
                    return null;
                }

                return Convert(data);
            }
        }

        public async Task<IEnumerable<FundingClaimsCollection>> GetCollectionsAsync(CancellationToken cancellationToken, int collectionYear)
        {
            using (var context = _fundingClaimsDataContext())
            {
                return await context.CollectionDetail.Where(f => f.CollectionYear == collectionYear)
                    .Select(x => Convert(x))
                    .ToListAsync(cancellationToken);
            }
        }

        public async Task<FundingClaimsCollection> GetLastUpdatedCollectionAsync(CancellationToken cancellationToken)
        {
            using (var context = _fundingClaimsDataContext())
            {

                var data = await context.CollectionDetail
                       .OrderByDescending(x => x.DateTimeUpdatedUtc)
                       .FirstOrDefaultAsync(cancellationToken);

                if (data == null)
                {
                    return null;
                }

                return Convert(data);
            }
        }

        public async Task<bool> UpdateCollection(CancellationToken cancellationToken, FundingClaimsCollection dto)
        {
            using (var context = _fundingClaimsDataContext())
            {
                var entity = await context.CollectionDetail.SingleOrDefaultAsync(x => x.CollectionId == dto.CollectionId, cancellationToken);
                if (entity == null)
                {
                    _logger.LogError($"Unable to find funding claims Collection metadata with id: {dto.CollectionName}");
                    return false;
                }

                entity.SubmissionOpenDateUtc = dto.SubmissionOpenDateUtc;
                entity.SubmissionCloseDateUtc = dto.SubmissionCloseDateUtc;
                entity.SignatureCloseDateUtc = dto.SignatureCloseDateUtc;
                entity.RequiresSignature = dto.RequiresSignature;
                entity.HelpdeskOpenDateUtc = dto.HelpdeskOpenDateUtc;
                entity.DateTimeUpdatedUtc = dto.DateTimeUpdatedUtc;
                entity.UpdatedBy = dto.UpdatedBy;

                await context.SaveChangesAsync(cancellationToken);
                return true;
            }
        }


        private FundingClaimsCollection Convert(CollectionDetail data)
        {
            var nowUtcDateTime = _dateTimeProvider.GetNowUtc();

            return new FundingClaimsCollection()
            {
                CollectionId = data.CollectionId,
                CollectionYear = data.CollectionYear,
                RequiresSignature = data.RequiresSignature.GetValueOrDefault(),
                SignatureCloseDateUtc = data.SignatureCloseDateUtc,
                SubmissionOpenDateUtc = data.SubmissionOpenDateUtc,
                SubmissionCloseDateUtc = data.SubmissionCloseDateUtc,
                CollectionName = data.CollectionName,
                SummarisedPeriodFrom = data.SummarisedPeriodFrom,
                SummarisedPeriodTo = data.SummarisedPeriodTo,
                SummarisedReturnPeriod = data.SummarisedReturnPeriod,
                DisplayName = data.DisplayTitle,
                IsOpenForSubmission = nowUtcDateTime >= data.SubmissionOpenDateUtc && nowUtcDateTime <= data.SubmissionCloseDateUtc,
                UpdatedBy = data.UpdatedBy,
                HelpdeskOpenDateUtc = data.HelpdeskOpenDateUtc,
                DateTimeUpdatedUtc = data.DateTimeUpdatedUtc
            };
        }
    }
}

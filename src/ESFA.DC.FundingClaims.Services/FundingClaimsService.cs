using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.FundingClaims.Data;
using ESFA.DC.FundingClaims.Data.Entities;
using ESFA.DC.FundingClaims.Message;
using ESFA.DC.FundingClaims.Model;
using ESFA.DC.FundingClaims.Model.Interfaces;
using ESFA.DC.FundingClaims.ReferenceData.Services.Interfaces;
using ESFA.DC.FundingClaims.Services.Interfaces;
using ESFA.DC.Logging.Interfaces;
using Microsoft.EntityFrameworkCore;
using FundingClaimsData = ESFA.DC.FundingClaims.Model.FundingClaimsData;

namespace ESFA.DC.FundingClaims.Services
{
    public class FundingClaimsService : IFundingClaimsService
    {
        private readonly Func<IFundingClaimsDataContext> _fundingClaimsContextFactory;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IFundingClaimsReferenceDataService _fundingClaimsReferenceDataService;
        private readonly ILogger _logger;

        public FundingClaimsService(
            Func<IFundingClaimsDataContext> fundingClaimsContextFactory,
            IDateTimeProvider dateTimeProvider,
            IFundingClaimsReferenceDataService fundingClaimsReferenceDataService,
            ILogger logger)
        {
            _fundingClaimsContextFactory = fundingClaimsContextFactory;
            _dateTimeProvider = dateTimeProvider;
            _fundingClaimsReferenceDataService = fundingClaimsReferenceDataService;
            _logger = logger;
        }

        public async Task<List<FundingClaimsDataItem>> GetSubmissionDetailsAsync(CancellationToken cancellationToken, long ukprn, Guid? submissionId = null, string collectionName = null)
        {
            var items = new List<FundingClaimsDataItem>();

            try
            {
                using (IFundingClaimsDataContext context = _fundingClaimsContextFactory())
                {
                    var submission = await GetSubmissionAsync(cancellationToken, context, ukprn, submissionId, collectionName);

                    if (submission == null)
                    {
                        _logger.LogDebug($"submission not found for submission id : {submissionId} and ukprn : {ukprn}");
                        return items;
                    }

                    items = await context.SubmissionValue.Where(x => x.SubmissionId == submission.SubmissionId)
                        .Select(x => new FundingClaimsDataItem()
                        {
                            ContractAllocationNumber = x.ContractAllocationNumber,
                            DeliverableCode = x.FundingStreamPeriodDeliverableCode.DeliverableCode,
                            DeliverableDescription = x.FundingStreamPeriodDeliverableCode.Description,
                            DeliveryToDate = x.DeliveryToDate,
                            ExceptionalAdjustments = x.ExceptionalAdjustments,
                            ForecastedDelivery = x.ForecastedDelivery,
                            FundingStreamPeriodCode = x.FundingStreamPeriodCode,
                            StudentNumbers = x.StudentNumbers,
                            TotalDelivery = x.TotalDelivery,
                        })
                        .ToListAsync(cancellationToken);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(
                    $"error getting submission detail for ukprn : {ukprn}, submission id : {submissionId} ", e);
                throw;
            }

            _logger.LogInfo($"return submission detail for ukprn : {ukprn}, submission id : {submissionId}");

            return items;
        }

        public async Task<bool> SaveSubmissionAsync(CancellationToken cancellationToken, FundingClaimsData fundingClaimsData)
        {
            try
            {
                using (IFundingClaimsDataContext context = _fundingClaimsContextFactory())
                {
                    var today = _dateTimeProvider.GetNowUtc();

                    var submission = await GetSubmissionAsync(cancellationToken, context, fundingClaimsData.Ukprn, null, fundingClaimsData.CollectionName, false);

                    var deliverableCodes = await context.FundingStreamPeriodDeliverableCode.ToListAsync(cancellationToken);

                    if (submission == null)
                    {
                        submission = new Submission
                        {
                            SubmissionId = Guid.NewGuid(),
                            Ukprn = fundingClaimsData.Ukprn,
                            Collection = await context.CollectionDetail.SingleOrDefaultAsync(x => x.CollectionName == fundingClaimsData.CollectionName, cancellationToken),
                            CreatedBy = fundingClaimsData.UserName,
                            CreatedDateTimeUtc = today,
                        };
                        context.Submission.Add(submission);
                    }
                    else
                    {
                        //find unique FSPs
                        var fundingStreamPeriodCodes = fundingClaimsData.FundingClaimsDataItems
                            .Select(x => x.FundingStreamPeriodCode).Distinct();

                        //find and remove values
                        context.SubmissionValue.RemoveRange(context.SubmissionValue.Where(x =>
                            x.SubmissionId == submission.SubmissionId &&
                            fundingStreamPeriodCodes.Contains(x.FundingStreamPeriodCode)));

                        context.ChangeLog.RemoveRange(
                            context.ChangeLog.Where(f => f.SubmissionId == submission.SubmissionId));

                        _logger.LogInfo(
                            $"removed funding claims draft data submission detail for ukprn : {fundingClaimsData.Ukprn}, collectionName: {fundingClaimsData.CollectionName}");
                    }

                    submission.IsSigned = false;
                    submission.SubmittedDateTimeUtc = null;
                    submission.SubmittedBy = null;
                    submission.SignedOnDateTimeUtc = null;
                    submission.Version = 0;

                    foreach (var value in fundingClaimsData.FundingClaimsDataItems)
                    {
                        var deliverableCode = deliverableCodes.Single(x => x.DeliverableCode == value.DeliverableCode && x.FundingStreamPeriodCode == value.FundingStreamPeriodCode);

                        submission.SubmissionValue.Add(new SubmissionValue()
                        {
                            SubmissionId = submission.SubmissionId,
                            ContractAllocationNumber = value.ContractAllocationNumber,
                            ExceptionalAdjustments = value.ExceptionalAdjustments.GetValueOrDefault(),
                            FundingStreamPeriodCode = value.FundingStreamPeriodCode,
                            ForecastedDelivery = value.ForecastedDelivery.GetValueOrDefault(),
                            DeliveryToDate = value.DeliveryToDate.GetValueOrDefault(),
                            StudentNumbers = value.StudentNumbers.GetValueOrDefault(),
                            TotalDelivery = value.TotalDelivery.GetValueOrDefault(),
                            FundingStreamPeriodDeliverableCode = deliverableCode,
                        });
                    }

                    submission.ChangeLog = new List<ChangeLog>
                      {
                        new ChangeLog
                        {
                            SubmissionId = submission.SubmissionId,
                            UserEmailAddress = fundingClaimsData.EmailAddress,
                            UpdatedDateTimeUtc = today,
                        },
                      };

                    await context.SaveChangesAsync(cancellationToken);

                    _logger.LogInfo(
                        $"saved funding claims draft data submission for ukprn : {fundingClaimsData.Ukprn}, collectionPerioId : {fundingClaimsData.CollectionName} ");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(
                    $"error getting submission detail for ukprn : {fundingClaimsData.Ukprn}, collectionPerioId id : {fundingClaimsData.CollectionName} ",
                    e);
                throw;
            }

            return true;
        }

        public async Task<List<ContractAllocation>> GetSubmittedMaxContractValuesAsync(CancellationToken cancellationToken, long ukprn, Guid submissionId)
        {
            var items = new List<ContractAllocation>();

            using (var context = _fundingClaimsContextFactory())
            {
                if (!(await context.Submission.AnyAsync(x => x.Ukprn == ukprn && x.SubmissionId == submissionId, cancellationToken)))
                {
                    throw new Exception($"submission does not belong to the provider : ukprn :{ukprn} , submission id : {submissionId}");
                }

                var contractAllocations = await context.SubmissionContractDetail.Where(x => x.SubmissionId == submissionId).ToListAsync(cancellationToken);

                foreach (var contractAllocation in contractAllocations)
                {
                    items.Add(new ContractAllocation()
                    {
                        FundingStreamPeriodCode = contractAllocation.FundingStreamPeriodCode,
                        MaximumContractValue = contractAllocation.ContractValue,
                    });
                }
            }

            return items;
        }

        public async Task<string> ConvertToSubmissionAsync(CancellationToken cancellationToken, long ukprn, string collectionName)
        {
            using (IFundingClaimsDataContext context = _fundingClaimsContextFactory())
            {
                var submission = await GetSubmissionAsync(cancellationToken, context, ukprn, null, collectionName);

                if (submission == null)
                {
                    throw new ArgumentException($"no submission found for submission for ukprn : {ukprn}, collection Name : {collectionName}, can not proceed with submission");
                }

                var contractsAllocations =
                    await _fundingClaimsReferenceDataService.GetContractAllocationsAsync(
                        cancellationToken,
                        submission.Ukprn,
                        submission.Collection.CollectionYear);

                var fundingStreamPeriodCodes = context.SubmissionValue
                    .Where(x => x.SubmissionId == submission.SubmissionId)
                    .Select(x => x.FundingStreamPeriodCode)
                    .Distinct();

                foreach (var value in fundingStreamPeriodCodes)
                {
                    var contract = contractsAllocations?.FirstOrDefault(x => x.FundingStreamPeriodCode.Equals(value));

                    if (contract != null)
                    {
                        submission.OrganisationIdentifier = contract.OrganisationIdentifier;

                        submission.SubmissionContractDetail.Add(new SubmissionContractDetail()
                        {
                            SubmissionId = submission.SubmissionId,
                            FundingStreamPeriodCode = value,
                            ContractValue = contract.MaximumContractValue,
                        });
                    }
                }

                // Mark as submitted
                submission.IsSubmitted = true;
                submission.SubmittedBy = submission.CreatedBy;
                submission.SubmittedDateTimeUtc = _dateTimeProvider.GetNowUtc();
                submission.Declaration = true;
                submission.Version = await GetLatestSubmissionVersionAsync(cancellationToken, ukprn);

                await context.SaveChangesAsync(cancellationToken);
                _logger.LogInfo($"Successfully converted to funding claims submission for submission Id :{submission.SubmissionId}");

                return submission.SubmissionId.ToString();
            }
        }

        public async Task<List<FundingClaimsSubmission>> GetSubmissionHistoryAsync(CancellationToken cancellationToken, long ukprn)
        {
            var result = new List<FundingClaimsSubmission>();

            try
            {
                using (var context = _fundingClaimsContextFactory())
                {
                    result = await context.Submission.Where(x => x.Ukprn == ukprn && x.IsSubmitted == true)
                        .OrderByDescending(x => x.SubmittedDateTimeUtc)
                        .Select(item => new FundingClaimsSubmission()
                        {
                            Ukprn = ukprn,
                            SubmissionId = item.SubmissionId.ToString(),
                            CollectionName = item.Collection.CollectionName,
                            SubmittedDateTime = item.SubmittedDateTimeUtc.GetValueOrDefault(),
                            IsSigned = item.IsSigned,
                            CollectionDisplayName = item.Collection.DisplayTitle,
                        })
                        .ToListAsync(cancellationToken);
                }

                return result.OrderByDescending(x => x.SubmittedDateTime).ToList();
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to get GetSubmissionHistoryAsync for ukprn : {ukprn}", e);
                throw;
            }
        }

        public async Task<int> GetLatestSubmissionVersionAsync(CancellationToken cancellationToken, long ukprn)
        {
            using (IFundingClaimsDataContext context = _fundingClaimsContextFactory())
            {
                var latestSubmission = await context.Submission
                    .Where(x => x.Ukprn == ukprn && x.IsSubmitted)
                    .MaxAsync(x => (int?)x.Version, cancellationToken);

                return latestSubmission.GetValueOrDefault() + 1;
            }
        }

        private async Task<Submission> GetSubmissionAsync(CancellationToken cancellationToken, IFundingClaimsDataContext context, long ukprn, Guid? submissionId = null, string collectionName = null, bool? isSubmitted = null)
        {
            var query = context.Submission.Include(x => x.Collection)
                .Where(x => x.Ukprn == ukprn);

            if (!string.IsNullOrEmpty(collectionName))
            {
                query = query.Where(x => x.Collection.CollectionName == collectionName);
            }

            if (submissionId != null)
            {
                query = query.Where(x => x.SubmissionId == submissionId);
            }

            if (isSubmitted != null)
            {
                query = query.Where(x => x.IsSubmitted == isSubmitted);
            }

            return await query.OrderByDescending(x => x.CreatedDateTimeUtc).FirstOrDefaultAsync(cancellationToken);
        }
    }
}

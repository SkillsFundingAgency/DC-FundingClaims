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
        private readonly IFundingClaimsMessagingService _fundingClaimsMessagingService;
        private readonly IIndex<int, IFundingStreamPeriodCodes> _fundingStreamPeriodCodes;
        private readonly IFundingClaimsReferenceDataService _fundingClaimsReferenceDataService;
        private readonly ICollectionReferenceDataService _collectionReferenceDataService;
        private readonly ILogger _logger;

        public FundingClaimsService(
            Func<IFundingClaimsDataContext> fundingClaimsContextFactory,
            IDateTimeProvider dateTimeProvider,
            IFundingClaimsMessagingService fundingClaimsMessagingService,
            IIndex<int, IFundingStreamPeriodCodes> IFundingStreamPeriodCodes,
            IFundingClaimsReferenceDataService fundingClaimsReferenceDataService,
            ICollectionReferenceDataService collectionReferenceDataService,
            ILogger logger)
        {
            _fundingClaimsContextFactory = fundingClaimsContextFactory;
            _dateTimeProvider = dateTimeProvider;
            _fundingClaimsMessagingService = fundingClaimsMessagingService;
            _fundingStreamPeriodCodes = IFundingStreamPeriodCodes;
            _fundingClaimsReferenceDataService = fundingClaimsReferenceDataService;
            _collectionReferenceDataService = collectionReferenceDataService;
            _logger = logger;
        }

        public async Task<List<Model.FundingClaimsDataItem>> GetDraftAsync(CancellationToken cancellationToken, string collectionCode, long ukprn)
        {
            var items = new List<Model.FundingClaimsDataItem>();

            try
            {
                using (IFundingClaimsDataContext context = _fundingClaimsContextFactory())
                {
                    foreach (var value in await context.FundingClaimsData.Where(x => x.Ukprn == ukprn && x.CollectionPeriod == collectionCode)
                        .ToListAsync(cancellationToken))
                    {
                        items.Add(new Model.FundingClaimsDataItem()
                        {
                            DeliverableCode = value.DeliverableCode,
                            DeliverableDescription = value.DeliverableDescription,
                            ExceptionalAdjustments = value.ExceptionalAdjustments,
                            FundingStreamPeriodCode = value.FundingStreamPeriodCode,
                            ForecastedDelivery = value.ForecastedDelivery,
                            DeliveryToDate = value.DeliveryToDate,
                            StudentNumbers = value.StudentNumbers,
                            TotalDelivery = value.TotalDelivery,
                        });
                    }
                }

                _logger.LogInfo($"returning draft items for ukprn :{ukprn}, count : {items.Count} ");
            }
            catch (Exception e)
            {
                _logger.LogError($"error getting draft values for ukprn : {ukprn} ", e);
                throw;
            }

            return items;
        }

        public async Task<List<Model.FundingClaimsDataItem>> GetSubmissionAsync(CancellationToken cancellationToken, Guid submissionId, long ukprn)
        {
            var items = new List<Model.FundingClaimsDataItem>();
            string ukprnStringValue = ukprn.ToString();

            try
            {
                using (IFundingClaimsDataContext context = _fundingClaimsContextFactory())
                {
                    if (!(await context.FundingClaimsSubmissionFile.
                        AnyAsync(x => x.Ukprn == ukprnStringValue && x.SubmissionId == submissionId, cancellationToken)))
                    {
                        throw new Exception($"submission does not belong to the provider : ukprn :{ukprn} , submission id : {submissionId}");
                    }

                    var data = await context.FundingClaimsSubmissionValues.Where(x => x.SubmissionId == submissionId).ToListAsync(cancellationToken);
                    foreach (var value in data)
                    {
                        items.Add(new Model.FundingClaimsDataItem()
                        {
                            DeliverableCode = value.DeliverableCode,
                            DeliverableDescription = value.DeliverableDescription,
                            ExceptionalAdjustments = value.ExceptionalAdjustments,
                            FundingStreamPeriodCode = value.FundingStreamPeriodCode,
                            ForecastedDelivery = value.ForecastedDelivery,
                            DeliveryToDate = value.DeliveryToDate,
                            StudentNumbers = value.StudentNumbers,
                            TotalDelivery = value.TotalDelivery,
                        });
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"error getting submission detail for ukprn : {ukprn}, submission id : {submissionId} ", e);
                throw;
            }

            _logger.LogInfo($"return submission detail for ukprn : {ukprn}, submission id : {submissionId}");

            return items;
        }

        public async Task<bool> SaveDraftAsync(CancellationToken cancellationToken, FundingClaimsData fundingClaimsData)
        {
            try
            {
                using (IFundingClaimsDataContext context = _fundingClaimsContextFactory())
                {
                    var submission = await context.Submission.SingleOrDefaultAsync(x => x.CollectionName == fundingClaimsData.CollectionName && x.Ukprn == fundingClaimsData.Ukprn && x.IsSubmitted == false, cancellationToken);

                    if (submission == null)
                    {
                        submission = new Submission
                        {
                            SubmissionId = Guid.NewGuid(),
                            Ukprn = fundingClaimsData.Ukprn,
                            CollectionName = fundingClaimsData.CollectionName,
                            CollectionYear = fundingClaimsData.CollectionYear,
                            CreatedBy = fundingClaimsData.UserName,
                            CreatedDateTimeUtc = _dateTimeProvider.GetNowUtc(),
                        };
                    }
                    else
                    {
                        //find unique FSPs
                        var fundingStreamPeriodCodes = fundingClaimsData.FundingClaimsDataItems
                            .Select(x => x.FundingStreamPeriodCode).Distinct();

                        //find and remove values
                        context.SubmissionValue.RemoveRange(context.SubmissionValue.Where(x => x.SubmissionId == submission.SubmissionId && fundingStreamPeriodCodes.Contains(x.FundingStreamPeriodCode)));

                        context.FundingClaimsLog.RemoveRange(context.FundingClaimsLog.Where(f => f.SubmissionId == submission.SubmissionId));

                        ////find and remove contract allocations
                        //context.SubmissionContractDetail.RemoveRange(context.SubmissionContractDetail.Where(x => x.SubmissionId == existingSubmission.SubmissionId && fundingStreamPeriodCodes.Contains(x.FundingStreamPeriodCode)));

                        _logger.LogInfo($"removed funding claims draft data submission detail for ukprn : {fundingClaimsData.Ukprn}, collectionName: {fundingClaimsData.CollectionName}");
                    }

                    submission.IsSigned = false;
                    submission.SubmittedDateTimeUtc = null;
                    submission.SubmittedBy = null;
                    submission.SignedOnDateTimeUtc = null;
                    submission.Version = 0;

                    foreach (var value in fundingClaimsData.FundingClaimsDataItems)
                    {
                        context.SubmissionValue.Add(new SubmissionValue()
                        {
                            ContractAllocationNumber = value.ContractAllocationNumber,
                            ExceptionalAdjustments = value.ExceptionalAdjustments.GetValueOrDefault(),
                            FundingStreamPeriodCode = value.FundingStreamPeriodCode,
                            ForecastedDelivery = value.ForecastedDelivery.GetValueOrDefault(),
                            DeliveryToDate = value.DeliveryToDate.GetValueOrDefault(),
                            StudentNumbers = value.StudentNumbers.GetValueOrDefault(),
                            TotalDelivery = value.TotalDelivery.GetValueOrDefault(),
                            DeliverableCodeId = value.DeliverableCode,
                        });
                    }

                    var today = _dateTimeProvider.GetNowUtc();
                    await context.FundingClaimsLog.AddAsync(
                        new FundingClaimsLog()
                        {
                            SubmissionId = submission.SubmissionId,
                            UserEmailAddress = fundingClaimsData.EmailAddress,
                            UpdatedDateTimeUtc = today,
                        }, cancellationToken);

                    await context.SaveChangesAsync(cancellationToken);

                    _logger.LogInfo($"saved funding claims draft data submission for ukprn : {fundingClaimsData.Ukprn}, collectionPerioId : {fundingClaimsData.CollectionName} ");
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"error getting submission detail for ukprn : {fundingClaimsData.Ukprn}, collectionPerioId id : {fundingClaimsData.CollectionName} ", e);
                throw;
            }

            return true;
        }

        public async Task<List<ContractAllocation>> GetSubmittedMaxContractValuesAsync(CancellationToken cancellationToken, long ukprn, Guid submissionId)
        {
            var items = new List<ContractAllocation>();

            using (var context = _fundingClaimsContextFactory())
            {
                if (!(await context.FundingClaimsSubmissionFile
                    .AnyAsync(x => x.Ukprn == ukprn.ToString() && x.SubmissionId == submissionId, cancellationToken)))
                {
                    throw new Exception($"submission does not belong to the provider : ukprn :{ukprn} , submission id : {submissionId}");
                }

                var contractAllocations = await context.FundingClaimMaxContractValues.Where(x => x.SubmissionId == submissionId).ToListAsync(cancellationToken);

                foreach (var contractAllocation in contractAllocations)
                {
                    items.Add(new ContractAllocation()
                    {
                        FundingStreamPeriodCode = contractAllocation.FundingStreamPeriodCode,
                        MaximumContractValue = contractAllocation.MaximumContractValue.GetValueOrDefault(),
                    });
                }
            }

            return items;
        }

        public async Task<string> ConvertToSubmissionAsync(CancellationToken cancellationToken, Guid submissionId)
        {
            using (IFundingClaimsDataContext context = _fundingClaimsContextFactory())
            {
                var submission =
                    await context.Submission.SingleOrDefaultAsync(
                        x => x.SubmissionId == submissionId && x.IsSubmitted == false, cancellationToken);

                if (submission == null)
                {
                    _logger.LogError($"submission id : {submissionId} not found, can not proceed with submission");
                    throw new ArgumentException(
                        $"submission id : {submissionId} not found, can not proceed with submission");
                }

                submission.IsSubmitted = true;
                submission.SubmittedDateTimeUtc = _dateTimeProvider.GetNowUtc();
                submission.Declaration = true;
                //submission.OrganisationIdentifier = orgDetails.Name

                var contractsAllocations =
                    await _fundingClaimsReferenceDataService.GetContractAllocationsAsync(
                        cancellationToken,
                        submission.Ukprn,
                        submission.CollectionYear);

                var fundingStreamPeriodCodes = context.SubmissionValue
                    .Where(x => x.SubmissionId == submissionId)
                    .Select(x => x.FundingStreamPeriodCode)
                    .Distinct();

                foreach (var value in fundingStreamPeriodCodes)
                {
                    var contract = contractsAllocations?.FirstOrDefault(x => x.FundingStreamPeriodCode.Equals(value));

                    if (contract != null)
                    {
                        context.SubmissionContractDetail.Add(new SubmissionContractDetail()
                        {
                            SubmissionId = submissionId,
                            FundingStreamPeriodCode = value,
                            ContractValue = contract.MaximumContractValue,
                        });
                    }
                }

                await context.SaveChangesAsync(cancellationToken);
            }

            _logger.LogInfo($"Successfully converted to funding claims submission for submission Id :{submissionId}");

            return submissionId.ToString();
        }
    }

    public async Task<List<FundingClaimsSubmission>> GetSubmissionHistoryAsync(CancellationToken cancellationToken, long ukprn)
    {
        var result = new List<FundingClaimsSubmission>();

        try
        {
            using (var context = _fundingClaimsContextFactory())
            {
                var collections = await _collectionReferenceDataService.GetAllFundingClaimsCollectionsAsync(cancellationToken);

                var data = await context.FundingClaimsSubmissionFile.Where(x => x.Ukprn == ukprn.ToString())
                    .OrderByDescending(x => x.UpdatedOn)
                    .ToListAsync(cancellationToken);

                foreach (var item in data)
                {
                    result.Add(new FundingClaimsSubmission()
                    {
                        Ukprn = ukprn,
                        SubmissionId = item.SubmissionId.ToString(),
                        CollectionCode = item.CollectionPeriod,
                        SubmittedDateTime = item.UpdatedOn,
                        IsSigned = item.IsSigned,
                        CollectionDisplayName = collections.SingleOrDefault(x => x.CollectionCode.Equals(item.CollectionPeriod))?.DisplayName,
                    });
                }
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
            var latestSubmission = await context.FundingClaimsSubmissionFile
                .Where(x => x.Ukprn == ukprn.ToString())
                .MaxAsync(x => (int?)x.Version, cancellationToken);

            return latestSubmission.GetValueOrDefault();
        }
    }
}
}

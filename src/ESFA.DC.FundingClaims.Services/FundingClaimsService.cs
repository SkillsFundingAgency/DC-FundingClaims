using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.FuncingClaims.Services.Interfaces;
using ESFA.DC.FundingClaims.Data;
using ESFA.DC.FundingClaims.Message;
using ESFA.DC.FundingClaims.Model;
using ESFA.DC.FundingClaims.Model.Interfaces;
using ESFA.DC.FundingClaims.ReferenceData.Services.Interfaces;
using ESFA.DC.Logging.Interfaces;
using Microsoft.EntityFrameworkCore;

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

        public async Task<List<Model.FundingClaimsDataItem>> GetDraftAsync(string collectionCode, long ukprn)
        {
            var items = new List<Model.FundingClaimsDataItem>();

            try
            {
                using (IFundingClaimsDataContext context = _fundingClaimsContextFactory())
                {
                    foreach (var value in await context.FundingClaimsData.Where(x => x.Ukprn == ukprn && x.CollectionPeriod == collectionCode).ToListAsync())
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
            }

            return items;
        }

        public async Task<List<Model.FundingClaimsDataItem>> GetSubmissionAsync(Guid submissionId, long ukprn)
        {
            var items = new List<Model.FundingClaimsDataItem>();

            try
            {
                using (IFundingClaimsDataContext context = _fundingClaimsContextFactory())
                {
                    if (!(await context.FundingClaimsSubmissionFile.AnyAsync(x =>
                        x.Ukprn == ukprn.ToString() && x.SubmissionId == submissionId)))
                    {
                        throw new Exception($"submission does not belong to the provider : ukprn :{ukprn} , submission id : {submissionId}");
                    }

                    var data = await context.FundingClaimsSubmissionValues.Where(x => x.SubmissionId == submissionId).ToListAsync();
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

        public async Task<bool> SaveDraftAsync(FundingClaimsData fundingClaimsData)
        {
            try
            {
                List<Data.Entities.FundingClaimsData> existingData = null;

                using (IFundingClaimsDataContext context = _fundingClaimsContextFactory())
                {
                    var ilr1619FundingStreamPeriodCode = _fundingStreamPeriodCodes[fundingClaimsData.AcademicYear].Ilr16To19;

                    if (fundingClaimsData.FundingClaimsDataItems.All(x => x.FundingStreamPeriodCode.Equals(ilr1619FundingStreamPeriodCode, StringComparison.OrdinalIgnoreCase)))
                    {
                        existingData = await context.FundingClaimsData.Where(x => x.CollectionPeriod == fundingClaimsData.CollectionCode && x.Ukprn == fundingClaimsData.Ukprn && x.FundingStreamPeriodCode.Equals(ilr1619FundingStreamPeriodCode, StringComparison.OrdinalIgnoreCase)).ToListAsync();
                    }
                    else
                    {
                        existingData = await context.FundingClaimsData.Where(x => x.CollectionPeriod == fundingClaimsData.CollectionCode && x.Ukprn == fundingClaimsData.Ukprn && !x.FundingStreamPeriodCode.Equals(ilr1619FundingStreamPeriodCode, StringComparison.OrdinalIgnoreCase)).ToListAsync();
                    }

                    if (existingData.Any())
                    {
                        context.FundingClaimsData.RemoveRange(existingData);
                        _logger.LogInfo($"removed funding claims draft data submission detail for ukprn : {fundingClaimsData.Ukprn}, collectionPerioId : {fundingClaimsData.CollectionCode} ");
                    }

                    foreach (var value in fundingClaimsData.FundingClaimsDataItems)
                    {
                        context.FundingClaimsData.Add(new Data.Entities.FundingClaimsData()
                        {
                            CollectionPeriod = fundingClaimsData.CollectionCode,
                            ContractAllocationNumber = value.ContractAllocationNumber,
                            DeliverableCode = value.DeliverableCode,
                            DeliverableDescription = value.DeliverableDescription,
                            ExceptionalAdjustments = value.ExceptionalAdjustments,
                            FundingStreamPeriodCode = value.FundingStreamPeriodCode,
                            ForecastedDelivery = value.ForecastedDelivery,
                            DeliveryToDate = value.DeliveryToDate,
                            StudentNumbers = value.StudentNumbers,
                            TotalDelivery = value.TotalDelivery,
                            Ukprn = fundingClaimsData.Ukprn,
                        });
                    }

                    context.FundingClaimsSupportingData.RemoveRange(
                        context.FundingClaimsSupportingData.Where(f => f.CollectionCode == fundingClaimsData.CollectionCode && f.Ukprn == fundingClaimsData.Ukprn));

                    var today = _dateTimeProvider.GetNowUtc();
                    await context.FundingClaimsSupportingData.AddAsync(new Data.Entities.FundingClaimsSupportingData()
                    {
                        CollectionCode = fundingClaimsData.CollectionCode,
                        Ukprn = fundingClaimsData.Ukprn,
                        UserEmailAddress = fundingClaimsData.EmailAddress,
                        LastUpdatedDateTimeUtc = today,
                    });

                    await context.SaveChangesAsync();

                    _logger.LogInfo($"saved funding claims draft data submission for ukprn : {fundingClaimsData.Ukprn}, collectionPerioId : {fundingClaimsData.CollectionCode} ");
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"error getting submission detail for ukprn : {fundingClaimsData.Ukprn}, collectionPerioId id : {fundingClaimsData.CollectionCode} ", e);
                throw;
            }

            return true;
        }

        public async Task<List<ContractAllocation>> GetSubmittedMaxContractValues(long ukprn, Guid submissionId)
        {
            var items = new List<ContractAllocation>();

            using (var context = _fundingClaimsContextFactory())
            {
                if (!(await context.FundingClaimsSubmissionFile.AnyAsync(x =>
                    x.Ukprn == ukprn.ToString() && x.SubmissionId == submissionId)))
                {
                    throw new Exception($"submission does not belong to the provider : ukprn :{ukprn} , submission id : {submissionId}");
                }

                var contractAllocations = await context.FundingClaimMaxContractValues.Where(x => x.SubmissionId == submissionId).ToListAsync();

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

        public async Task<string> ConvertToSubmissionAsync(long ukprn, int latestSubmissionVersion, string collectionCode, int academicYear)
        {
            var orgDetails = await _fundingClaimsReferenceDataService.GetorganisationDetails(ukprn);

            if (orgDetails == null)
            {
                throw new Exception($"ukprn is not valid : {ukprn}");
            }

            var currentDateTime = _dateTimeProvider.GetNowUtc();

            var contractsAllocations = await _fundingClaimsReferenceDataService.GetContractAllocationsAsync(ukprn, academicYear);
            using (IFundingClaimsDataContext context = _fundingClaimsContextFactory())
            {
                var draftFundingClaims = await context.FundingClaimsData.Where(x => x.Ukprn == ukprn && x.CollectionPeriod == collectionCode).ToListAsync();

                _logger.LogInfo($"Got draft values for funding ");
                var submissionId = Guid.NewGuid();

                foreach (var value in draftFundingClaims)
                {
                    var contract = contractsAllocations?.FirstOrDefault(x =>
                        x.FundingStreamPeriodCode.Equals(value.FundingStreamPeriodCode));

                    if (contract != null)
                    {
                        if (!context.FundingClaimMaxContractValues.Local.Any(x => x.SubmissionId == submissionId && x.FundingStreamPeriodCode == value.FundingStreamPeriodCode))
                        {
                            context.FundingClaimMaxContractValues.Add(new Data.Entities.FundingClaimMaxContractValues()
                            {
                                SubmissionId = submissionId,
                                FundingStreamPeriodCode = value.FundingStreamPeriodCode,
                                MaximumContractValue = contract.MaximumContractValue,
                            });
                        }

                        if (!context.FundingClaimsSubmissionFile.Local.Any(x => x.SubmissionId == submissionId))
                        {
                            context.FundingClaimsSubmissionFile.Add(new Data.Entities.FundingClaimsSubmissionFile()
                            {
                                SubmissionId = submissionId,
                                Period = contract.Period,
                                Ukprn = ukprn.ToString(),
                                PeriodTypeCode = contract.PeriodTypeCode,
                                OrganisationIdentifier = contract.OrganisationIdentifier,
                                CollectionPeriod = collectionCode,
                                IsHeProvider = orgDetails.IsHesaProvider,
                                Declaration = true,
                                SubmissionType = 2,
                                ProviderName = orgDetails.Name,
                                Allb24PlsMaximumContractValue = 0m,
                                AsbMaximumContractValue = 0m,
                                ClContractValue = 0m,
                                DlsMaximumContractValue = 0m,
                                Version = latestSubmissionVersion + 1,
                                UpdatedOn = currentDateTime,
                                UpdatedBy = orgDetails.Name,
                            });
                        }

                        context.FundingClaimsSubmissionValues.Add(new Data.Entities.FundingClaimsSubmissionValues()
                        {
                            CollectionPeriod = collectionCode,
                            ContractAllocationNumber = value.ContractAllocationNumber,
                            DeliverableCode = value.DeliverableCode,
                            DeliverableDescription = value.DeliverableDescription,
                            ExceptionalAdjustments = value.ExceptionalAdjustments.GetValueOrDefault(),
                            FundingStreamPeriodCode = value.FundingStreamPeriodCode,
                            ForecastedDelivery = value.ForecastedDelivery.GetValueOrDefault(),
                            DeliveryToDate = value.DeliveryToDate.GetValueOrDefault(),
                            StudentNumbers = value.StudentNumbers.GetValueOrDefault(),
                            TotalDelivery = value.TotalDelivery.GetValueOrDefault(),
                            SubmissionId = submissionId,
                        });
                    }
                }

                await context.SaveChangesAsync();

                _logger.LogInfo($"Successfully converted to funding claims submission for submission Id :{submissionId}");

                try
                {
                    await _fundingClaimsMessagingService.SendMessageAsync(
                            new MessageFundingClaimsSubmission
                            {
                                SubmissionId = submissionId,
                            });
                }
                catch (Exception e)
                {
                    _logger.LogError($"Failed to send message to funding claims queue for submission Id :{submissionId}", e);
                }

                return submissionId.ToString();
            }
        }

        public async Task<List<FundingClaimsSubmission>> GetSubmissionHistoryAsync(long ukprn)
        {
            var result = new List<FundingClaimsSubmission>();

            try
            {
                using (var context = _fundingClaimsContextFactory())
                {
                    var collections = await _collectionReferenceDataService.GetAllFundingClaimsCollections();

                    var data = await context.FundingClaimsSubmissionFile.Where(x => x.Ukprn == ukprn.ToString())
                        .OrderByDescending(x => x.UpdatedOn)
                        .ToListAsync();

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

        public async Task<int> GetLatestSubmissionVersion(long ukprn)
        {
            using (IFundingClaimsDataContext context = _fundingClaimsContextFactory())
            {
                var latestSubmission = await context.FundingClaimsSubmissionFile
                    .Where(x => x.Ukprn == ukprn.ToString())
                    .MaxAsync(x => (int?)x.Version);

                return latestSubmission.GetValueOrDefault();
            }
        }
    }
}

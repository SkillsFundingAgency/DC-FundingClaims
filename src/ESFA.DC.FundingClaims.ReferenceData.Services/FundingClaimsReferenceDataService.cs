using ESFA.DC.FundingClaims.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FundingClaims.Model.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.ReferenceData.FCS.Model.Interface;
using ESFA.DC.ReferenceData.Organisations.Model.Interface;
using ESFA.DC.Summarisation.Model.Interface;
using Microsoft.EntityFrameworkCore;
using ESFA.DC.FundingClaims.Data;
using Autofac.Features.Indexed;
using ESFA.DC.FundingClaims.ReferenceData.Services.Interfaces;
using ESFA.DC.ILR1920.DataStore.EF.Interface;
using ESFA.DC.ILR1920.DataStore.EF;

namespace ESFA.DC.FundingClaims.ReferenceData.Services
{
    public class FundingClaimsReferenceDataService : IFundingClaimsReferenceDataService
    {
        private readonly Func<IFcsContext> _fcsContextFactory;
        private readonly Func<IILR1920_DataStoreEntities> _ilr1920RulebaseContextFactory;
        private readonly Func<IOrganisationsContext> _organisationContextFactory;
        private readonly Func<IFundingClaimsDataContext> _fundingClaimsContextFactory;
        private readonly IIndex<int, IFundingStreamPeriodCodes> _fundingStreamPeriodCodes;
        private readonly Func<ISummarisationContext> _summarisedActualsContextFactory;
        private readonly ILogger _logger;

        private readonly Dictionary<string, int> rateBandsDisctionary =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                {"540+ hours (Band 5)", 1},
                {"450+ hours (Band 4a)", 2},
                {"450 to 539 hours (Band 4b)", 3},
                {"360 to 449 hours (Band 3)", 4},
                {"280 to 359 hours (Band 2)", 5},
                {"Up to 279 hours (Band 1)", 6}
            };

    public FundingClaimsReferenceDataService(
            Func<IFcsContext> fcsContextFactory,
            Func<IILR1920_DataStoreEntities> ilr1920RulebaseContextFactory,
            Func<IOrganisationsContext> organisationContextFactory,
            Func<IFundingClaimsDataContext> fundingClaimsContextFactory,
            IIndex<int, IFundingStreamPeriodCodes> IFundingStreamPeriodCodes,
            Func<ISummarisationContext> summarisedActualsContextFactory,
            ILogger logger)
        {
            _fcsContextFactory = fcsContextFactory;
            _ilr1920RulebaseContextFactory = ilr1920RulebaseContextFactory;
            _organisationContextFactory = organisationContextFactory;
            _fundingClaimsContextFactory = fundingClaimsContextFactory;
            _fundingStreamPeriodCodes = IFundingStreamPeriodCodes;
            _summarisedActualsContextFactory = summarisedActualsContextFactory;
            _logger = logger;
        }

        public async Task<List<ContractAllocation>> GetContractAllocationsAsync(CancellationToken cancellationToken, long ukprn, int collectionYear)
        {
            var items = new List<ContractAllocation>();
            var stringCollectionYear = collectionYear.ToString();

            using (var context = _fcsContextFactory())
            {
                var contractAllocations = await context.ContractAllocations
                    .Where(x => x.DeliveryUkprn == ukprn && x.Period == stringCollectionYear).ToListAsync(cancellationToken);

                //all contracts
                foreach (var contractAllocation in contractAllocations)
                {
                    items.Add(ConvertContractAllocation(contractAllocation, contractAllocation.MaxContractValue));
                }
            }

            return items;
        }

        public async Task<List<Ilr16To19FundingClaim>> Get1619FundingClaimDetailsAsync(CancellationToken cancellationToken, long ukprn)
        {
            List<Ilr16To19FundingClaim> result = new List<Ilr16To19FundingClaim>();

            try
            {
                using (var context = _ilr1920RulebaseContextFactory())
                {
                    var mainData = await context.FM25_Learners
                        .Where(x => x.UKPRN == ukprn && x.StartFund.GetValueOrDefault()).ToListAsync(cancellationToken);

                    var data1 = GetLineByLineFundingClaims(
                        mainData.Where(x => x.FundLine.Equals(
                            "14-16 Direct Funded Students",
                            StringComparison.InvariantCultureIgnoreCase)),
                        "14-16 Direct Funded Students",
                        1);
                    var data2 = GetLineByLineFundingClaims(
                        mainData.Where(x =>
                            x.FundLine.Equals(
                                "16-19 Students (excluding High Needs Students)",
                                StringComparison.InvariantCultureIgnoreCase)
                            || x.FundLine.Equals(
                                "16-19 High Needs Students",
                                StringComparison.InvariantCultureIgnoreCase)),
                        "16-19 Students (including High Needs Students)",
                        2);

                    var data3 = GetLineByLineFundingClaims(
                        mainData.Where(x => x.FundLine.Equals(
                            "19-24 Students with an EHCP",
                            StringComparison.InvariantCultureIgnoreCase)),
                        "19-24 Students with an EHCP",
                        3);
                    var data4 = GetLineByLineFundingClaims(
                        mainData.Where(x => x.FundLine.Equals(
                            "19+ Continuing Students (excluding EHCP)",
                            StringComparison.InvariantCultureIgnoreCase)), "19+ Continuing Students (excluding EHCP)",
                        4);

                    result.AddRange(data1);
                    result.AddRange(data2);
                    result.AddRange(data3);
                    result.AddRange(data4);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to get Get1619FundingClaimDetailsAsync for ukprn : {ukprn}", e);
                throw;
            }

            return result.OrderBy(x => x.FundLineSortOrder).ThenBy(x => x.RateBandSortOrder).ToList();
        }

        public async Task<ProviderReferenceData> GetProviderReferenceDataAsync(CancellationToken cancellationToken, long ukprn, int collectionYear)
        {
            var stringCollectionYear = collectionYear.ToString();
            string aebNonProcuredPeriodCode = _fundingStreamPeriodCodes[collectionYear].AdultEducationBudgetNonProcured;

            var result = new ProviderReferenceData();


            using (var context = _fcsContextFactory())
            {
                //community learning
                var data = await context.ContractDeliverables
                    .FirstOrDefaultAsync(x =>
                    x.ContractAllocation.DeliveryUkprn == ukprn &&
                    x.ContractAllocation.Period == stringCollectionYear &&
                    x.DeliverableCode == 5 &&
                    x.ContractAllocation.FundingStreamPeriodCode == aebNonProcuredPeriodCode, cancellationToken);
                if (data != null)
                {
                    result.AebcClallocation = data.PlannedValue.GetValueOrDefault();
                }
            }

            return result;
        }

        public async Task<ProviderReferenceData> GetProviderReferenceDataAsync(CancellationToken cancellationToken, long ukprn)
        {
            try
            {
                using (IFundingClaimsDataContext context = _fundingClaimsContextFactory())
                {
                    var data = await context.FundingClaimsProviderReferenceData.FirstOrDefaultAsync(x =>
                        x.Ukprn == ukprn, cancellationToken);
                    if (data != null)
                    {
                        return new ProviderReferenceData()
                        {
                            AebcClallocation = data.AebcClallocation,
                            EditAccess = data.EditAccess,
                        };
                    }
                    else
                    {
                        return new ProviderReferenceData();
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to get GetProviderRefernceDataAsync for ukprn : {ukprn}", e);
                throw;
            }
        }

        public async Task<IEnumerable<SummarisedActualDeliveryToDate>> GetDeliveryToDateValuesAsync(
                                                CancellationToken cancellationToken, 
                                                long ukprn, 
                                                int periodFrom, 
                                                int periodTo, 
                                                string collectionReturnCode, 
                                                int collectionYear)
        {
            try
            {
                string organisationIdentifier;

                using (var context = _fcsContextFactory())
                {
                    organisationIdentifier =
                        (await context.ContractAllocations.Where(x => x.DeliveryUkprn == ukprn).FirstOrDefaultAsync(cancellationToken))
                        ?.DeliveryOrganisation;
                }

                var fundingStreamPeriodCodes = _fundingStreamPeriodCodes[collectionYear];

                IEnumerable<SummarisedActualDeliveryToDate> result = null;

                using (var context = _summarisedActualsContextFactory())
                {
                    result = context.SummarisedActuals.Where(x => x.OrganisationId == organisationIdentifier &&
                                                                  x.Period >= periodFrom && x.Period <= periodTo &&
                                                                  x.CollectionReturn.CollectionReturnCode == collectionReturnCode &&
                                                                  (
                                                                      (x.FundingStreamPeriodCode == fundingStreamPeriodCodes.AdultEducationBudgetNonProcured &&
                                                                       x.DeliverableCode >= 2 && x.DeliverableCode <= 6) ||
                                                                      (x.FundingStreamPeriodCode == fundingStreamPeriodCodes.AdvanceLoadBursary &&
                                                                       x.DeliverableCode >= 2 && x.DeliverableCode <= 4) ||
                                                                      (x.FundingStreamPeriodCode == fundingStreamPeriodCodes.TraineeshipNonProcured &&
                                                                       x.DeliverableCode >= 2 && x.DeliverableCode <= 3)))
                        .GroupBy(x => new { x.FundingStreamPeriodCode, x.DeliverableCode })
                        .ToList()
                        .Select(x => new SummarisedActualDeliveryToDate()
                        {
                            DeliverableCode = x.Key.DeliverableCode,
                            FundingStreamPeriodCode = x.Key.FundingStreamPeriodCode,
                            DeliveryToDate = x.Sum(y => y.ActualValue),
                        })
                        .OrderBy(x => x.FundingStreamPeriodCode)
                        .ThenBy(x => x.DeliverableCode);
                }

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(
                    $"error occured to GetDeliveryToDateValues data for ukprn : {ukprn}  period from: {periodFrom}, period to : {periodTo}, collection return code :{collectionReturnCode} ",
                    e);
                return null;
            }
        }

        public async Task<decimal?> GetCofRemovalValueAsync(CancellationToken cancellationToken, long ukprn)
        {
            try
            {
                var startDate = new DateTime(2019, 8, 1);
                var endDate = new DateTime(2020, 7, 31);

                using (var context = _organisationContextFactory())
                {
                    var data = await context.ConditionOfFundingRemovals.FirstOrDefaultAsync(x =>
                        x.Ukprn == ukprn && x.EffectiveFrom >= startDate && x.EffectiveTo <= endDate, cancellationToken) ;

                    return data?.CoFremoval;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"error occured to GetCofRemovalValue data for ukprn : {ukprn}", e);
                return null;
            }
        }

        private ContractAllocation ConvertContractAllocation(ESFA.DC.ReferenceData.FCS.Model.ContractAllocation contractAllocation, decimal? allocationValue)
        {
            return new ContractAllocation()
            {
                ContractAllocationNumber = contractAllocation.ContractAllocationNumber,
                FundingStreamPeriodCode = contractAllocation.FundingStreamPeriodCode,
                MaximumContractValue = allocationValue.GetValueOrDefault(),
                Period = contractAllocation.Period,
                PeriodTypeCode = contractAllocation.PeriodTypeCode,
                OrganisationIdentifier = contractAllocation.DeliveryOrganisation,
            };
        }

        private IEnumerable<Ilr16To19FundingClaim> GetLineByLineFundingClaims(IEnumerable<FM25_Learner> data, string fundLine, int fundLineSortOrder)
        {
            var result = data.GroupBy(x => new
            {
                x.RateBand,
            })
            .Select(x => new Ilr16To19FundingClaim
            {
                FundLine = fundLine,
                RateBand = x.Key.RateBand,
                StudentNumbers = x.Select(p => p.LearnRefNumber).Count(),
                DeliveryToDate = x.Sum(p => p.OnProgPayment),
                RateBandSortOrder = GetRateBandSortOrder(x.Key.RateBand),
                FundLineSortOrder = fundLineSortOrder,
            }).ToList();

            return result;
        }

        private int GetRateBandSortOrder(string rateBand)
        {
            int result = 0;
            if (rateBandsDisctionary.TryGetValue(rateBand, out result))
            {
                return result;
            }
            
            return 7;
        }

      
    }
}

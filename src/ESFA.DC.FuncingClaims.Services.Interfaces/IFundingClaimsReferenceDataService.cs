using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.FundingClaims.Model;
using ESFA.DC.ReferenceData.Organisations.Model;

namespace ESFA.DC.FuncingClaims.Services.Interfaces
{
    public interface IFundingClaimsReferenceDataService
    {
        Task<List<ContractAllocation>> GetContractAllocationsAsync(long ukprn, int collectionYear);

        Task<List<Ilr16To19FundingClaim>> Get1619FundingClaimDetailsAsync(long ukprn);

        Task<OrgDetail> GetorganisationDetails(long ukprn);

        Task<ProviderReferenceData> GetProviderRefernceDataAsync(long ukprn);

        Task<ProviderReferenceData> GetProviderRefernceDataAsync(long ukprn, int collectionYear);

        Task<IEnumerable<SummarisedActualDeliveryToDate>> GetDeliveryToDateValues(long ukprn, int periodFrom, int periodTo, string collectionReturnCode, int collectionYear);

        Task<decimal?> GetCofRemovalValue(long ukprn);
    }
}

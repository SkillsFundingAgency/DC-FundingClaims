using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FundingClaims.Model;

namespace ESFA.DC.FundingClaims.ReferenceData.Services.Interfaces
{
    public interface IFundingClaimsReferenceDataService
    {
        Task<List<ContractAllocation>> GetContractAllocationsAsync(CancellationToken cancellationToken, long ukprn, int collectionYear);

        Task<List<Ilr16To19FundingClaim>> Get1619FundingClaimDetailsAsync(CancellationToken cancellationToken, long ukprn);

        Task<ProviderReferenceData> GetProviderReferenceDataAsync(CancellationToken cancellationToken, long ukprn);

        Task<ProviderReferenceData> GetProviderReferenceDataAsync(CancellationToken cancellationToken, long ukprn, int collectionYear);

        Task<IEnumerable<SummarisedActualDeliveryToDate>> GetDeliveryToDateValuesAsync(CancellationToken cancellationToken, long ukprn, int periodFrom, int periodTo, string collectionReturnCode, int collectionYear);

        Task<decimal?> GetCofRemovalValueAsync(CancellationToken cancellationToken, long ukprn);
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.FundingClaims.Model;

namespace ESFA.DC.FundingClaims.ReferenceData.Services.Interfaces
{
    public interface IFundingClaimsReferenceDataService
    {
        Task<List<ContractAllocation>> GetContractAllocationsAsync(long ukprn, int collectionYear);

        Task<List<Ilr16To19FundingClaim>> Get1619FundingClaimDetailsAsync(long ukprn);

        Task<ProviderDetails> GetorganisationDetails(long ukprn);

        Task<ProviderReferenceData> GetProviderRefernceDataAsync(long ukprn);

        Task<ProviderReferenceData> GetProviderRefernceDataAsync(long ukprn, int collectionYear);

        Task<IEnumerable<SummarisedActualDeliveryToDate>> GetDeliveryToDateValues(long ukprn, int periodFrom, int periodTo, string collectionReturnCode, int collectionYear);

        Task<decimal?> GetCofRemovalValue(long ukprn);

        Task<FundingClaimsCollection> GetFundingClaimsCollection(string collectionCode);

        Task<FundingClaimsCollection> GetFundingClaimsCollection(DateTime? dateTimeUtc = null);


        Task<List<FundingClaimsCollection>> GetAllFundingClaimsCollections();

        Task<string> GetEmailTemplate(int collectionId);
    }
}

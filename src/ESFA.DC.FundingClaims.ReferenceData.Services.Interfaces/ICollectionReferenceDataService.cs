using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FundingClaims.Model;

namespace ESFA.DC.FundingClaims.ReferenceData.Services.Interfaces
{
    public interface ICollectionReferenceDataService
    {
        Task<FundingClaimsCollection> GetFundingClaimsCollectionAsync(CancellationToken cancellationToken, string collectionCode);

        Task<List<FundingClaimsCollection>> GetAllFundingClaimsCollectionsAsync(CancellationToken cancellationToken);

        Task<FundingClaimsCollection> GetFundingClaimsCollectionAsync(CancellationToken cancellationToken, DateTime? dateTimeUtc = null);

        Task<FundingClaimsCollection> GetLatestFundingClaimsCollectionAsync(CancellationToken cancellationToken, bool? requiresSignature = null);

        Task<IEnumerable<FundingClaimsCollection>> GetCollectionsAsync(CancellationToken cancellationToken,
            int collectionYear);

        Task<FundingClaimsCollection> GetLastUpdatedCollectionAsync(CancellationToken cancellationToken);

        Task<bool> UpdateCollection(CancellationToken cancellationToken, FundingClaimsCollection dto);
    }
}

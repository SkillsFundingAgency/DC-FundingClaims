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

        Task<string> GetEmailTemplateAsync(CancellationToken cancellationToken, int collectionId);

        Task<FundingClaimsCollection> GetLatestFundingClaimsCollectionAsync(CancellationToken cancellationToken, bool requiresSignature);
    }
}

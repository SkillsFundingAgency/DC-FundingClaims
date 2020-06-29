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
        Task<FundingClaimsCollection> GetFundingClaimsCollection(string collectionCode);

        Task<List<FundingClaimsCollection>> GetAllFundingClaimsCollections();

        Task<FundingClaimsCollection> GetFundingClaimsCollection(DateTime? dateTimeUtc = null);

        Task<string> GetEmailTemplate(int collectionId);

        Task<FundingClaimsCollection> GetLatestFundingClaimsCollectionAsync(CancellationToken cancellationToken, bool requiresSignature);
    }
}

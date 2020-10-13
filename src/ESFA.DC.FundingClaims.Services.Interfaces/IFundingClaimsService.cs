using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FundingClaims.Model;

namespace ESFA.DC.FundingClaims.Services.Interfaces
{
    public interface IFundingClaimsService
    {
        Task<bool> SaveSubmissionAsync(CancellationToken cancellationToken, FundingClaimsData fundingClaimsData);

        Task<FundingClaimsData> GetSubmissionDetailsAsync(CancellationToken cancellationToken, long ukprn, Guid? submissionId = null, string collectionName = null);

        Task<string> ConvertToSubmissionAsync(CancellationToken cancellationToken, long ukprn, string collectionName);

        Task<List<FundingClaimsSubmission>> GetSubmissionHistoryAsync(CancellationToken cancellationToken, long ukprn);

        Task<List<ContractAllocation>> GetSubmittedMaxContractValuesAsync(CancellationToken cancellationToken, long ukprn, Guid submissionId);

        Task<int> GetLatestSubmissionVersionAsync(CancellationToken cancellationToken, long ukprn);

        Task UpdateCovidDeclaration(CancellationToken cancellationToken, long ukprn, string collectionName, bool? value);
    }
}

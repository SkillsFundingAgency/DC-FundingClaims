using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FundingClaims.Model;

namespace ESFA.DC.FundingClaims.Services.Interfaces
{
    public interface IFundingClaimsService
    {
        Task<bool> SaveDraftAsync(CancellationToken cancellationToken, FundingClaimsData fundingClaimsData);

        Task<List<FundingClaimsDataItem>> GetDraftAsync(CancellationToken cancellationToken, string collectionCode, long ukprn);

        Task<List<FundingClaimsDataItem>> GetSubmissionAsync(CancellationToken cancellationToken, Guid submissionId, long ukprn);

        Task<string> ConvertToSubmissionAsync(CancellationToken cancellationToken, long ukprn, int latestSubmissionVersion, string collectionName, int academicYear);

        Task<List<FundingClaimsSubmission>> GetSubmissionHistoryAsync(CancellationToken cancellationToken, long ukprn);

        Task<List<ContractAllocation>> GetSubmittedMaxContractValuesAsync(CancellationToken cancellationToken, long ukprn, Guid submissionId);

        Task<int> GetLatestSubmissionVersionAsync(CancellationToken cancellationToken, long ukprn);
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.FundingClaims.Model;

namespace ESFA.DC.FundingClaims.Services.Interfaces
{
    public interface IFundingClaimsService
    {
        Task<bool> SaveDraftAsync(FundingClaimsData fundingClaimsData);

        Task<List<FundingClaimsDataItem>> GetDraftAsync(string collectionCode, long ukprn);

        Task<List<FundingClaimsDataItem>> GetSubmissionAsync(Guid submissionId, long ukprn);

        Task<string> ConvertToSubmissionAsync(long ukprn, int latestSubmissionVersion, string collectionName, int academicYear);

        Task<List<FundingClaimsSubmission>> GetSubmissionHistoryAsync(long ukprn);

        Task<List<ContractAllocation>> GetSubmittedMaxContractValues(long ukprn, Guid submissionId);

        Task<int> GetLatestSubmissionVersion(long ukprn);
    }
}

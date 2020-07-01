using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FundingClaims.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.FundingClaims.Data
{
    public interface IFundingClaimsDataContext : IDisposable
    {
        DbSet<FundingClaimDetails> FundingClaimDetails { get; set; }

        DbSet<FundingClaimMaxContractValues> FundingClaimMaxContractValues { get; set; }

        DbSet<FundingClaimsData> FundingClaimsData { get; set; }

        DbSet<FundingClaimsProviderReferenceData> FundingClaimsProviderReferenceData { get; set; }

        DbSet<FundingClaimsSubmissionFile> FundingClaimsSubmissionFile { get; set; }

        DbSet<FundingClaimsSubmissionValues> FundingClaimsSubmissionValues { get; set; }

        DbSet<SubmissionTypes> SubmissionTypes { get; set; }

        DbSet<FundingClaimsSupportingData> FundingClaimsSupportingData { get; set; }

        DbSet<SigningNotificationFeed> SigningNotificationFeed { get; set; }

        int SaveChanges();

        System.Threading.Tasks.Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry Entry(object entity);
    }
}

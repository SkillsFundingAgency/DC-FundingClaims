﻿using System;
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
        DbSet<FundingStreamPeriodDeliverableCode> FundingStreamPeriodDeliverableCode { get; set; }

        DbSet<ChangeLog> ChangeLog { get; set; }

        DbSet<FundingClaimsProviderReferenceData> FundingClaimsProviderReferenceData { get; set; }

        DbSet<SigningNotificationFeed> SigningNotificationFeed { get; set; }

        DbSet<Submission> Submission { get; set; }

        DbSet<SubmissionContractDetail> SubmissionContractDetail { get; set; }

        DbSet<SubmissionValue> SubmissionValue { get; set; }

        DbSet<CollectionDetail> CollectionDetail { get; set; }

        int SaveChanges();

        System.Threading.Tasks.Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry Entry(object entity);
    }
}

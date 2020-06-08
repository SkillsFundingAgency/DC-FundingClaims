using ESFA.DC.FundingClaims.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.FundingClaims.Data
{
    public partial class FundingClaimsDataContext : DbContext, IFundingClaimsDataContext
    {
        public FundingClaimsDataContext()
        {
        }

        public FundingClaimsDataContext(DbContextOptions<FundingClaimsDataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<FundingClaimDetails> FundingClaimDetails { get; set; }

        public virtual DbSet<FundingClaimMaxContractValues> FundingClaimMaxContractValues { get; set; }

        public virtual DbSet<FundingClaimsData> FundingClaimsData { get; set; }

        public virtual DbSet<FundingClaimsFieldRule> FundingClaimsFieldRule { get; set; }

        public virtual DbSet<FundingClaimsProviderReferenceData> FundingClaimsProviderReferenceData { get; set; }

        public virtual DbSet<FundingClaimsSubmissionFile> FundingClaimsSubmissionFile { get; set; }

        public virtual DbSet<FundingClaimsSubmissionValues> FundingClaimsSubmissionValues { get; set; }

        public virtual DbSet<SubmissionTypes> SubmissionTypes { get; set; }

        public virtual DbSet<FundingClaimsSupportingData> FundingClaimsSupportingData { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLOCALDB;Database=FundingClaims;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FundingClaimDetails>(entity =>
            {
                entity.HasKey(e => e.DataCollectionKey);

                entity.Property(e => e.DataCollectionKey)
                    .HasMaxLength(50)
                    .ValueGeneratedNever();

                entity.Property(e => e.SignatureCloseDate).HasColumnType("datetime");

                entity.Property(e => e.SubmissionCloseDate).HasColumnType("datetime");

                entity.Property(e => e.SubmissionOpenDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<FundingClaimMaxContractValues>(entity =>
            {
                entity.Property(e => e.FundingStreamPeriodCode).HasMaxLength(50);

                entity.Property(e => e.MaximumContractValue).HasColumnType("decimal(16, 2)");
            });

            modelBuilder.Entity<FundingClaimsData>(entity =>
            {
                entity.ToTable("FundingClaimsData", "Draft");

                entity.Property(e => e.CollectionPeriod)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ContractAllocationNumber)
                    .HasColumnName("contractAllocationNumber")
                    .HasMaxLength(100);

                entity.Property(e => e.DeliverableDescription)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.DeliveryToDate).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.ExceptionalAdjustments).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.ForecastedDelivery).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.FundingStreamPeriodCode).HasMaxLength(50);

                entity.Property(e => e.TotalDelivery).HasColumnType("decimal(12, 2)");
            });

            modelBuilder.Entity<FundingClaimsSupportingData>(entity =>
            {
                entity.HasKey(e => new { e.Ukprn, e.CollectionCode });

                entity.ToTable("FundingClaimsSupportingData", "Draft");

                entity.Property(e => e.CollectionCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Ukprn)
                    .IsRequired()
                    .HasColumnName("UKPRN");

                entity.Property(e => e.UserEmailAddress)
                    .IsRequired()
                    .HasColumnType("nvarchar(320)");

                entity.Property(e => e.LastUpdatedDateTimeUtc)
                    .IsRequired()
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<FundingClaimsFieldRule>(entity =>
            {
                entity.Property(e => e.DataCollectionKey)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.FundingStreamPeriodCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.MappedColumnName)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<FundingClaimsProviderReferenceData>(entity =>
            {
                entity.HasKey(e => e.Ukprn);

                entity.ToTable("FundingClaimsProviderReferenceData", "Reference");

                entity.Property(e => e.Ukprn)
                    .HasColumnName("UKPRN")
                    .ValueGeneratedNever();

                entity.Property(e => e.AebcClallocation)
                    .HasColumnName("AEBC-CLAllocation")
                    .HasColumnType("decimal(10, 2)");
            });

            modelBuilder.Entity<FundingClaimsSubmissionFile>(entity =>
            {
                entity.HasKey(e => new { e.SubmissionId, e.CollectionPeriod, e.SubmissionType });

                entity.Property(e => e.CollectionPeriod).HasMaxLength(50);

                entity.Property(e => e.Allb24PlsMaximumContractValue).HasColumnType("decimal(16, 2)");

                entity.Property(e => e.AsbMaximumContractValue).HasColumnType("decimal(16, 2)");

                entity.Property(e => e.ClContractValue).HasColumnType("decimal(16, 2)");

                entity.Property(e => e.DlsMaximumContractValue).HasColumnType("decimal(16, 2)");

                entity.Property(e => e.OrganisationIdentifier)
                    .HasColumnName("organisationIdentifier")
                    .HasMaxLength(100);

                entity.Property(e => e.Period)
                    .HasColumnName("period")
                    .HasMaxLength(100);

                entity.Property(e => e.PeriodTypeCode)
                    .HasColumnName("periodTypeCode")
                    .HasMaxLength(100);

                entity.Property(e => e.ProviderName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.SignedOn).HasColumnType("datetime");

                entity.Property(e => e.Ukprn)
                    .IsRequired()
                    .HasColumnName("UKPRN")
                    .HasMaxLength(10);

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

                entity.HasOne(d => d.SubmissionTypeNavigation)
                    .WithMany(p => p.FundingClaimsSubmissionFile)
                    .HasForeignKey(d => d.SubmissionType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SubmissionType");
            });

            modelBuilder.Entity<FundingClaimsSubmissionValues>(entity =>
            {
                entity.Property(e => e.CollectionPeriod)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ContractAllocationNumber)
                    .HasColumnName("contractAllocationNumber")
                    .HasMaxLength(100);

                entity.Property(e => e.DeliverableDescription)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.DeliveryToDate).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.ExceptionalAdjustments).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.ForecastedDelivery).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.FundingStreamPeriodCode).HasMaxLength(50);

                entity.Property(e => e.TotalDelivery).HasColumnType("decimal(12, 2)");
            });

            modelBuilder.Entity<SubmissionTypes>(entity =>
            {
                entity.ToTable("SubmissionTypes", "Static");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });
        }
    }
}

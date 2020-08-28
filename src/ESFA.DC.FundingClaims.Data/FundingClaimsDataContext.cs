using ESFA.DC.FundingClaims.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.FundingClaims.Data
{
    public partial class FundingClaimsDataContext : DbContext
    {
        public FundingClaimsDataContext()
        {
        }

        public FundingClaimsDataContext(DbContextOptions<FundingClaimsDataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<DeliverableCode> DeliverableCode { get; set; }

        public virtual DbSet<FundingClaimsData> FundingClaimsData { get; set; }

        public virtual DbSet<FundingClaimsLog> FundingClaimsLog { get; set; }

        public virtual DbSet<FundingClaimsProviderReferenceData> FundingClaimsProviderReferenceData { get; set; }

        public virtual DbSet<SigningNotificationFeed> SigningNotificationFeed { get; set; }

        public virtual DbSet<Submission> Submission { get; set; }

        public virtual DbSet<SubmissionContractDetail> SubmissionContractDetail { get; set; }

        public virtual DbSet<SubmissionValue> SubmissionValue { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=.;Database=FundingClaims_New;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<DeliverableCode>(entity =>
            {
                entity.Property(e => e.DeliverableCodeId).ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(1000);
            });

            modelBuilder.Entity<FundingClaimsData>(entity =>
            {
                entity.ToTable("FundingClaimsData", "Draft");

                entity.Property(e => e.CollectionName)
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

            modelBuilder.Entity<FundingClaimsLog>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CollectionName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UpdatedDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.UserEmailAddress)
                    .IsRequired()
                    .HasMaxLength(320);
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

            modelBuilder.Entity<SigningNotificationFeed>(entity =>
            {
                entity.Property(e => e.DateTimeUpdatedUtc).HasColumnType("datetime");

                entity.Property(e => e.FeedDateTimeUtc).HasColumnType("datetime");
            });

            modelBuilder.Entity<Submission>(entity =>
            {
                entity.Property(e => e.SubmissionId).ValueGeneratedNever();

                entity.Property(e => e.CollectionName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.CreatedDateTimeUtc)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.OrganisationIdentifier).HasMaxLength(100);

                entity.Property(e => e.ProviderName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.SignedOnDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.SubmittedBy).HasMaxLength(250);

                entity.Property(e => e.SubmittedDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.Ukprn).HasColumnName("UKPRN");
            });

            modelBuilder.Entity<SubmissionContractDetail>(entity =>
            {
                entity.Property(e => e.ContractAllocationNumber)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.ContractValue).HasColumnType("decimal(16, 2)");

                entity.Property(e => e.FundingStreamPeriodCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Submission)
                    .WithMany(p => p.SubmissionContractDetail)
                    .HasForeignKey(d => d.SubmissionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SubmissionContractDetail_Submission");
            });

            modelBuilder.Entity<SubmissionValue>(entity =>
            {
                entity.Property(e => e.DeliveryToDate).HasColumnType("decimal(16, 2)");

                entity.Property(e => e.ExceptionalAdjustments).HasColumnType("decimal(16, 2)");

                entity.Property(e => e.ForecastedDelivery).HasColumnType("decimal(16, 2)");

                entity.Property(e => e.TotalDelivery).HasColumnType("decimal(16, 2)");

                entity.HasOne(d => d.DeliverableCode)
                    .WithMany(p => p.SubmissionValue)
                    .HasForeignKey(d => d.DeliverableCodeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SubmissionValue_DeliverableCode");

                entity.HasOne(d => d.Submission)
                    .WithMany(p => p.SubmissionValue)
                    .HasForeignKey(d => d.SubmissionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SubmissionValue_Submission");
            });
        }
    }
}

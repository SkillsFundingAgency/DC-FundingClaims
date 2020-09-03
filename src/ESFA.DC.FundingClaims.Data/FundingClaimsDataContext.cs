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

        public virtual DbSet<ChangeLog> ChangeLog { get; set; }

        public virtual DbSet<CollectionDetail> CollectionDetail { get; set; }

        public virtual DbSet<DeliverableCode> DeliverableCode { get; set; }

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
                optionsBuilder.UseSqlServer("Server=.;Database=FC_TST_NEW;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<ChangeLog>(entity =>
            {
                entity.Property(e => e.UpdatedDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.UserEmailAddress)
                    .IsRequired()
                    .HasMaxLength(320);

                entity.HasOne(d => d.Submission)
                    .WithMany(p => p.ChangeLog)
                    .HasForeignKey(d => d.SubmissionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ChangeLog_Submission");
            });

            modelBuilder.Entity<CollectionDetail>(entity =>
            {
                entity.HasIndex(e => e.CollectionId)
                    .HasName("UQ__Collecti__7DE6BC05C85E8FAD")
                    .IsUnique();

                entity.Property(e => e.CollectionCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CollectionName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.DateTimeUpdatedUtc)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(CONVERT([datetime],'01 JAN 1900'))");

                entity.Property(e => e.DisplayTitle)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.HelpdeskOpenDateUtc)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(CONVERT([datetime],'01 JAN 1900'))");

                entity.Property(e => e.SignatureCloseDateUtc).HasColumnType("datetime");

                entity.Property(e => e.SubmissionCloseDateUtc).HasColumnType("datetime");

                entity.Property(e => e.SubmissionOpenDateUtc).HasColumnType("datetime");

                entity.Property(e => e.SummarisedReturnPeriod)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('DataMigration')");
            });

            modelBuilder.Entity<DeliverableCode>(entity =>
            {
                entity.Property(e => e.DeliverableCodeId).HasColumnName("DeliverableCodeId");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.FundingStreamPeriodCode)
                    .IsRequired()
                    .HasMaxLength(50);
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

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.CreatedDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.OrganisationIdentifier).HasMaxLength(100);

                entity.Property(e => e.SignedOnDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.SubmittedBy).HasMaxLength(250);

                entity.Property(e => e.SubmittedDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.Ukprn).HasColumnName("UKPRN");

                entity.HasOne(d => d.Collection)
                    .WithMany(p => p.Submission)
                    .HasPrincipalKey(p => p.CollectionId)
                    .HasForeignKey(d => d.CollectionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Submission_CollectionDetail");
            });

            modelBuilder.Entity<SubmissionContractDetail>(entity =>
            {
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
                entity.Property(e => e.ContractAllocationNumber)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DeliveryToDate).HasColumnType("decimal(16, 2)");

                entity.Property(e => e.ExceptionalAdjustments).HasColumnType("decimal(16, 2)");

                entity.Property(e => e.ForecastedDelivery).HasColumnType("decimal(16, 2)");

                entity.Property(e => e.FundingStreamPeriodCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

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

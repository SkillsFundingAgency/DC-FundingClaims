using ESFA.DC.ReferenceData.FCS.Model;
using ESFA.DC.ReferenceData.FCS.Model.Interface;
using ESFA.DC.ReferenceData.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.ReferenceData.FCS.Service.Config
{
    public class FcsDbContextFactory : IDbContextFactory<IFcsReadWriteContext>
    {
        private readonly string _connectionString;

        public FcsDbContextFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IFcsReadWriteContext Create()
        {
            DbContextOptions<FcsContext> options =
                new DbContextOptionsBuilder<FcsContext>()
                .UseSqlServer(_connectionString).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options;

            return new FcsContext(options);
        }
    }
}

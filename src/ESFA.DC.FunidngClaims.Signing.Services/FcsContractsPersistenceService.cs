using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.ReferenceData.FCS.Model;
using ESFA.DC.ReferenceData.FCS.Model.Interface;
using ESFA.DC.ReferenceData.FCS.Service.Interface;
using ESFA.DC.ReferenceData.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.ReferenceData.FCS.Service
{
    public class FcsContractsPersistenceService : IFcsContractsPersistenceService
    {
        private readonly IDbContextFactory<IFcsReadWriteContext> _fcsContextFactory;
        private readonly ILogger _logger;

        public FcsContractsPersistenceService(IDbContextFactory<IFcsReadWriteContext> fcsContextFactory, ILogger logger)
        {
            _fcsContextFactory = fcsContextFactory;
            _logger = logger;
        }

        public async Task<IEnumerable<Guid>> GetExistingSyndicationItemIds(CancellationToken cancellationToken)
        {
            using (var fcsContext = _fcsContextFactory.Create())
            {
                var syndicationItemIds = await fcsContext
                .Contractors
                .Select(c => c.SyndicationItemId)
                .ToListAsync(cancellationToken);

                return syndicationItemIds
                    .Where(c => c.HasValue)
                    .Select(c => c.Value)
                    .Distinct();
            }
        }

        public async Task PersistContracts(IEnumerable<Contractor> contractors, CancellationToken cancellationToken)
        {
            using (var fcsContext = _fcsContextFactory.Create())
            {
                using (var transaction = fcsContext.Database.BeginTransaction())
                {
                    try
                    {
                        contractors = contractors.ToList();

                        var contractNumbers = contractors.SelectMany(c => c.Contracts).Select(c => c.ContractNumber).ToList();

                        var defunctContractors = fcsContext
                            .Contractors
                            .Where(o => o.Contracts.Any(c => contractNumbers.Contains(c.ContractNumber)))
                            .ToList();

                        _logger.LogVerbose($"FCS Contracts - Persisting {contractors.Count()} Contractors - Removing {defunctContractors.Count()} Contractors");

                        fcsContext.Contractors.RemoveRange(defunctContractors);

                        fcsContext.Contractors.AddRange(contractors);

                        await fcsContext.SaveChangesAsync(cancellationToken);

                        transaction.Commit();

                        _logger.LogVerbose($"FCS Contracts - Persisted {contractors.Count()} Contractors - Removed {defunctContractors.Count()} Contractors");
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError("FCS Contracts Persist Failed", exception);

                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}

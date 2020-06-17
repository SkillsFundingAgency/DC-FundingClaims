using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.ReferenceData.FCS.Service.Config.Interface;
using ESFA.DC.ReferenceData.FCS.Service.Interface;
using ESFA.DC.ReferenceData.Interfaces;
using ESFA.DC.ReferenceData.Interfaces.Constants;

namespace ESFA.DC.ReferenceData.FCS.Service
{
    public class FcsReferenceDataTask : IReferenceDataTask
    {
        private readonly IFcsServiceConfiguration _fcsServiceConfiguration;
        private readonly IFcsFeedService _fcsFeedService;
        private readonly IFcsContractsPersistenceService _fcsContractsPersistenceService;
        private readonly ILogger _logger;

        public FcsReferenceDataTask(IFcsServiceConfiguration fcsServiceConfiguration, IFcsFeedService fcsFeedService, IFcsContractsPersistenceService fcsContractsPersistenceService, ILogger logger)
        {
            _fcsServiceConfiguration = fcsServiceConfiguration;
            _fcsFeedService = fcsFeedService;
            _fcsContractsPersistenceService = fcsContractsPersistenceService;
            _logger = logger;
        }

        public async Task ExecuteAsync(IReferenceDataTaskContext referenceDataTaskContext, CancellationToken cancellationToken)
        {
            try
            {
                var existingSyndicationItemIds = await _fcsContractsPersistenceService.GetExistingSyndicationItemIds(cancellationToken);

                var fcsContracts = await _fcsFeedService.GetNewContractorsFromFeedAsync(_fcsServiceConfiguration.FeedUri + "/api/contracts/notifications/approval-onwards", existingSyndicationItemIds, cancellationToken);

                await _fcsContractsPersistenceService.PersistContracts(fcsContracts, cancellationToken);
            }
            catch (Exception exception)
            {
                _logger.LogError("FCS Contracts Reference Data Task Failed", exception);
                throw;
            }
        }
    }
}

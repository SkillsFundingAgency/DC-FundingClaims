using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.FundingClaims.ReferenceData.Services.Interfaces;
using ESFA.DC.FundingClaims.Signing.Services.Interfaces;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.FundingClaims.Signing.Services
{
    public class NotificationCalendarService : INotificationCalendarService
    {
        private readonly ICollectionReferenceDataService _collectionReferenceDataService;
        private readonly IFeedRepository _feedRepository;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILogger _logger;

        public NotificationCalendarService(
            ICollectionReferenceDataService collectionReferenceDataService,
            IFeedRepository feedRepository,
            IDateTimeProvider dateTimeProvider,
            ILogger logger)
        {
            _collectionReferenceDataService = collectionReferenceDataService;
            _feedRepository = feedRepository;
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
        }

        public async Task<bool> IsSigningNotificationPollRequiredAsync(CancellationToken cancellationToken)
        {
            var currentDateTime = _dateTimeProvider.GetNowUtc();

            var latestCollection = await _collectionReferenceDataService.GetLatestFundingClaimsCollectionAsync(cancellationToken, true);

            if (latestCollection == null || currentDateTime >= latestCollection.SignatureCloseDateUtc.GetValueOrDefault().AddDays(1))
            {
                _logger.LogInfo("no available collection which require signin possibly one day elapsed since signature closed");
                return false;
            }

            if (latestCollection.IsOpenForSubmission)
            {
                _logger.LogInfo("Collection is still open , no feed poll required");
                return false;
            }

            if (currentDateTime > latestCollection.SubmissionCloseDateUtc &&  currentDateTime <= latestCollection.SignatureCloseDateUtc.GetValueOrDefault())
            {
                _logger.LogInfo("Collection is still closed and we are between submission close and signature close , feed poll will should be called");
                return true;
            }

            var latestFeedEntry = await _feedRepository.GetLatestSyndicationDataAsync(cancellationToken);

            if (latestFeedEntry != null && latestCollection.SignatureCloseDateUtc.GetValueOrDefault().AddHours(1) <= latestFeedEntry.DateTimeUpdatedUtc)
            {
                _logger.LogInfo("Collection is closed and signature closed date is passed too, we are in the grace period of one hour, feed poll will be done");
                return true;
            }

            _logger.LogInfo("Collection is closed and signature closed date is passed too, grace period of one hour is elapsed - No more feed poll required");
            return false;
        }
    }
}

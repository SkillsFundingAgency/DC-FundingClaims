using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FunidngClaims.Signing.Services.Interfaces;
using Quartz;

namespace ESFA.DC.FundingClaims.Signing.Notifications.Console
{
    public class SigningFeedJob : IJob
    {
        private readonly INotificationCalendarService _notificationCalendarService;
        private readonly IFundingClaimsFeedService _fundingClaimsFeedService;

        public SigningFeedJob(INotificationCalendarService notificationCalendarService, IFundingClaimsFeedService fundingClaimsFeedService)
        {
            _notificationCalendarService = notificationCalendarService;
            _fundingClaimsFeedService = fundingClaimsFeedService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            if (await _notificationCalendarService.IsSigningNotificationPollRequiredAsync(CancellationToken.None))
            {
                await _fundingClaimsFeedService.ExecuteAsync(CancellationToken.None);
            }
        }
    }
}

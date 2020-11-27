using System.Threading.Tasks;
using ESFA.DC.FundingClaims.Message;
using ESFA.DC.FundingClaims.Services.Interfaces;
using ESFA.DC.Queueing.Interface;

namespace ESFA.DC.FundingClaims.Services
{
    public class FundingClaimsMessagingService : IFundingClaimsMessagingService
    {
        private readonly IQueuePublishService<MessageFundingClaimsSubmission> _queuePublishService;

        public FundingClaimsMessagingService(IQueuePublishService<MessageFundingClaimsSubmission> queuePublishService)
        {
            _queuePublishService = queuePublishService;
        }

        public async Task SendMessageAsync(MessageFundingClaimsSubmission submissionMessage)
        {
            await _queuePublishService.PublishAsync(submissionMessage);
        }
    }
}

using System.Threading.Tasks;
using ESFA.DC.FundingClaims.Message;

namespace ESFA.DC.FuncingClaims.Services.Interfaces
{
    public interface IFundingClaimsMessagingService
    {
        Task SendMessageAsync(MessageFundingClaimsSubmission submissionMessage);
    }
}

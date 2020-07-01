using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.FundingClaims.Signing.Services.Interfaces
{
    public interface INotificationCalendarService
    {
        Task<bool> IsSigningNotificationPollRequiredAsync(CancellationToken cancellationToken);
    }
}

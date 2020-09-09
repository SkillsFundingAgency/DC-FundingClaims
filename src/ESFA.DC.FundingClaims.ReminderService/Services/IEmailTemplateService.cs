using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.FundingClaims.ReminderService.Services
{
    public interface IEmailTemplateService
    {
        Task<string> GetEmailTemplateAsync(CancellationToken cancellationToken, int collectionId);
    }
}

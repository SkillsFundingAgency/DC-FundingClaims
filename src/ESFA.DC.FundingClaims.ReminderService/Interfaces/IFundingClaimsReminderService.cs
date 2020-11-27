using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.FundingClaims.ReminderService.Interfaces
{
    public interface IFundingClaimsReminderService
    {
        Task Execute(CancellationToken cancellationToken);
    }
}

using System.Threading.Tasks;

namespace ESFA.DC.FundingClaims.EmailNotification.Services
{

    public interface IEmailNotifier
    {
        Task<string> SendEmail(string toEmail, string templateId, System.Collections.Generic.Dictionary<string, dynamic> parameters);
    }
}

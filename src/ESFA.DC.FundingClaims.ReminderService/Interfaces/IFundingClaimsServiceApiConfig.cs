using ESFA.DC.FundingClaims.EmailNotification.Services;

namespace ESFA.DC.FundingClaims.ReminderService.Interfaces
{
    public interface IFundingClaimsServiceApiConfig
    {
        string FundingClaimsApiEndPoint { get; }

        string LogConnectionString { get; }

        INotifierConfig NotifierConfig{ get; }
    }
}

using ESFA.DC.JobNotifications;

namespace ESFA.DC.FundingClaims.ReminderService.Interfaces
{
    public interface IFundingClaimsServiceApiConfig
    {
        string FundingClaimsApiEndPoint { get; }

        string LogConnectionString { get; }

        INotifierConfig NotifierConfig{ get; }
    }
}

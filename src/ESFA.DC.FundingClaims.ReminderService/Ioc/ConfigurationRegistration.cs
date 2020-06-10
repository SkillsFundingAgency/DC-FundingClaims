using Autofac;
using ESFA.DC.FundingClaims.EmailNotification.Services;
using ESFA.DC.FundingClaims.ReminderService.Configuration;
using Microsoft.Extensions.Configuration;

namespace ESFA.DC.FundingClaims.ReminderService.Ioc
{
    public static class ConfigurationRegistration
    {
        public static void SetupConfigurations(this ContainerBuilder builder, IConfiguration configuration)
        {
            builder.Register(c => configuration.GetConfigSection<ConnectionStrings>())
                            .As<ConnectionStrings>().SingleInstance();
            builder.Register(c => configuration.GetConfigSection<NotifierConfig>())
                .As<INotifierConfig>().SingleInstance();

        }
    }
}
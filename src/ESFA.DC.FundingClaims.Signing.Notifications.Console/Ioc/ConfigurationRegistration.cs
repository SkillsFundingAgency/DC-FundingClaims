using Autofac;
using ESFA.DC.FundingClaims.AtomFeed.Services.Config;
using ESFA.DC.FundingClaims.Signing.Notifications.Console.Configuration;
using Microsoft.Extensions.Configuration;

namespace ESFA.DC.FundingClaims.Signing.Notifications.Console.Ioc
{
    public static class ConfigurationRegistration
    {
        public static void SetupConfigurations(this ContainerBuilder builder, IConfiguration configuration)
        {
            builder.Register(c => configuration.GetConfigSection<ConnectionStrings>())
                            .As<ConnectionStrings>().SingleInstance();
            builder.Register(c => configuration.GetConfigSection<AtomFeedSettings>())
                .As<AtomFeedSettings>().SingleInstance();
        }
    }
}
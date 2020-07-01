using Autofac;
using ESFA.DC.FundingClaim.Console.Settings;
using Microsoft.Extensions.Configuration;

namespace ESFA.DC.FundingClaim.Console.Ioc
{
    public static class ConfigurationRegistration
    {
        public static void SetupConfigurations(this ContainerBuilder builder, IConfiguration configuration)
        {
            builder.Register(c => configuration.GetConfigSection<ConnectionStrings>())
                            .As<ConnectionStrings>().SingleInstance();

            builder.Register(c => configuration.GetConfigSection<FundingClaimsQueueConfiguration>())
                .As<FundingClaimsQueueConfiguration>().SingleInstance();
        }
    }
}
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.FundingClaims.ReminderService.Interfaces;
using ESFA.DC.FundingClaims.ReminderService.Ioc;
using ESFA.DC.Logging.Interfaces;
using ExecutionContext = ESFA.DC.Logging.ExecutionContext;

namespace ESFA.DC.FundingClaims.ReminderService
{
    public static class Program
    {
#if DEBUG
        private const string ConfigFile = "privatesettings.json";
#else
        private const string ConfigFile = "appsettings.json";
#endif

        public static async Task Main(string[] args)
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(ConfigFile);

            var configuration = configBuilder.Build();
            var containerBuilder = new ContainerBuilder();
            var configurationModule = new Autofac.Configuration.ConfigurationModule(configuration);

            containerBuilder.RegisterModule(configurationModule);
            containerBuilder.SetupConfigurations(configuration);
            containerBuilder.RegisterModule<ServiceRegistrations>();
            var container = containerBuilder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                var logger = scope.Resolve<ILogger>();
                logger.LogInfo($"Starting funding claim reminder service web job.");

                var fundingClaimsReminderService = scope.Resolve<IFundingClaimsReminderService>();
                await fundingClaimsReminderService.Execute(CancellationToken.None);
            }
        }
    }
}

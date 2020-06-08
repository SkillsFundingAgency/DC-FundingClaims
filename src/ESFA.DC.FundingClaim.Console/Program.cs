using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.FundingClaim.Console.Interfaces;
using ESFA.DC.FundingClaim.Console.Ioc;
using ESFA.DC.FundingClaims.Message;
using ESFA.DC.Logging.Interfaces;
using Microsoft.Extensions.Configuration;

namespace ESFA.DC.FundingClaim.Console
{
    public static class Program
    {
        public static CancellationToken CancellationToken = CancellationToken.None;

        public static async Task Main(string[] args)
        {
            var containerBuilder = new ContainerBuilder();

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory());

            var userSettingsFileName = $"appsettings.local.json";
            if (File.Exists($"{Directory.GetCurrentDirectory()}/{userSettingsFileName}"))
            {
                config.AddJsonFile(userSettingsFileName);
            }
            else
            {
                config.AddJsonFile("appsettings.json");
            }

            var configurationBuilder = config.Build();
            var configurationModule = new Autofac.Configuration.ConfigurationModule(configurationBuilder);

            containerBuilder.RegisterModule(configurationModule);
            containerBuilder.SetupConfigurations(configurationBuilder);
            containerBuilder.RegisterModule<ServiceRegistrations>();
            containerBuilder.RegisterModule<LoggerRegistrations>();
            var container = containerBuilder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                var logger = scope.Resolve<ILogger>();
                logger.LogInfo($"Starting funding claim web job.");

                var scheduler = scope.Resolve<IFundingClaimsWebJob<MessageFundingClaimsSubmission>>();
                scheduler.Subscribe();

                ManualResetEvent oSignalEvent = new ManualResetEvent(false);
                oSignalEvent.WaitOne();
                logger.LogInfo($"Finishing funding claim web job.");
            }
        }
    }
}
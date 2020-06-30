using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.FundingClaims.Signing.Noticifications.Console.Ioc;
using ESFA.DC.FunidngClaims.Signing.Services.Interfaces;
using ESFA.DC.Logging.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace ESFA.DC.FundingClaims.Signing.Noticifications.Console
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

                var pollRequiredService = scope.Resolve<INotificationCalendarService>();
                if (await pollRequiredService.IsSigningNotificationPollRequiredAsync(CancellationToken.None))
                {
                    var service = scope.Resolve<IFundingClaimsFeedService>();
                    await service.ExecuteAsync(CancellationToken.None);
                }

            }
        }
    }
}

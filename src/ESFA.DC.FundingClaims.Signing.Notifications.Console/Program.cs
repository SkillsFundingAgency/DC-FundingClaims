using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.Quartz;
using ESFA.DC.FundingClaims.Signing.Notifications.Console.Ioc;
using Microsoft.Extensions.Configuration;
using Quartz;

namespace ESFA.DC.FundingClaims.Signing.Notifications.Console
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
            containerBuilder.RegisterModule(new QuartzAutofacFactoryModule());
            containerBuilder.RegisterModule(new QuartzAutofacJobsModule(typeof(SigningFeedJob).Assembly));

            var container = containerBuilder.Build();

            var job = JobBuilder.Create<SigningFeedJob>().WithIdentity("SigningFeedJob", "Jobs").Build();
            var trigger = TriggerBuilder.Create()
                .WithIdentity("SigningFeedJob", "Jobs")
                .StartNow()
                .WithSchedule(SimpleScheduleBuilder.RepeatMinutelyForever(15)).Build();

            var cts = new CancellationTokenSource();

            var scheduler = container.Resolve<IScheduler>();
            await scheduler.ScheduleJob(job, trigger, cts.Token).ConfigureAwait(true);
            await scheduler.Start(cts.Token).ConfigureAwait(true);

            System.Console.ReadLine();

            cts.Cancel();
            await scheduler.Shutdown(cts.Token).ConfigureAwait(true);
        }
    }
}


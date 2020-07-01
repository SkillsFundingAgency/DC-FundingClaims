using System;
using System.Collections.Generic;
using Autofac;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.FundingClaims.Services.Interfaces;
using ESFA.DC.FundingClaims.Data;
using ESFA.DC.FundingClaims.EmailNotification.Services;
using ESFA.DC.FundingClaims.ReminderService.Configuration;
using ESFA.DC.FundingClaims.ReminderService.Interfaces;
using ESFA.DC.FundingClaims.Services;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.Logging;
using ESFA.DC.Logging.Config;
using ESFA.DC.Logging.Config.Interfaces;
using ESFA.DC.Logging.Enums;
using ESFA.DC.Logging.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.FundingClaims.ReminderService.Ioc
{
    public class ServiceRegistrations : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<JobQueueDataContext>().As<IJobQueueDataContext>().ExternallyOwned();
            builder.RegisterType<FundingClaimsDataContext>().As<IFundingClaimsDataContext>().ExternallyOwned();
            builder.RegisterType<EmailNotifier>().As<IEmailNotifier>().InstancePerLifetimeScope();
            builder.RegisterType<FundingClaimsReminderService>().As<IFundingClaimsReminderService>().InstancePerLifetimeScope();
            builder.RegisterType<FundingClaimsEmailService>().As<IFundingClaimsEmailService>().InstancePerLifetimeScope();
            builder.RegisterType<DateTimeProvider.DateTimeProvider>().As<IDateTimeProvider>().InstancePerLifetimeScope();

            builder.Register(c =>
            {
                var connectionStrings = c.Resolve<ConnectionStrings>();
                return new ApplicationLoggerSettings
                {
                    ApplicationLoggerOutputSettingsCollection = new List<IApplicationLoggerOutputSettings>()
                    {
                        new MsSqlServerApplicationLoggerOutputSettings()
                        {
                            MinimumLogLevel = LogLevel.Information,
                            ConnectionString = connectionStrings.AppLogs,
                        },
                    },
                    TaskKey = "Funding Claims Reminder",
                    JobId = "Funding Claims Reminder Service",
                };
            }).As<IApplicationLoggerSettings>().SingleInstance();

            builder.RegisterType<ExecutionContext>().As<IExecutionContext>().InstancePerLifetimeScope();
            builder.RegisterType<SerilogLoggerFactory>().As<ISerilogLoggerFactory>().InstancePerLifetimeScope();
            builder.RegisterType<SeriLogger>().As<ILogger>().InstancePerLifetimeScope();

            builder.RegisterType<FundingClaimsDataContext>().As<IFundingClaimsDataContext>().ExternallyOwned();
            builder.Register(context =>
                {
                    var connectionString = context.Resolve<ConnectionStrings>();
                    var optionsBuilder = new DbContextOptionsBuilder<FundingClaimsDataContext>();
                    optionsBuilder.UseSqlServer(
                        connectionString.FundingClaims,
                        options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));

                    return optionsBuilder.Options;
                })
                .As<DbContextOptions<FundingClaimsDataContext>>()
                .SingleInstance();

            builder.Register(context =>
                {
                    var optionsBuilder = new DbContextOptionsBuilder<JobQueueDataContext>();
                    var connectionString = context.Resolve<ConnectionStrings>();

                    optionsBuilder.UseSqlServer(
                        connectionString.JobManagement,
                        options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));

                    return optionsBuilder.Options;
                })
                .As<DbContextOptions<JobQueueDataContext>>()
                .SingleInstance();
        }
    }
}

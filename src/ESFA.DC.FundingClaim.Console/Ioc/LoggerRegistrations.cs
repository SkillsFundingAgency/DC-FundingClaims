using System.Collections.Generic;
using Autofac;
using ESFA.DC.FundingClaim.Console.Settings;
using ESFA.DC.Logging;
using ESFA.DC.Logging.Config;
using ESFA.DC.Logging.Config.Interfaces;
using ESFA.DC.Logging.Enums;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.FundingClaim.Console.Ioc
{
    public class LoggerRegistrations : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c =>
            {
                var connectionStrings = c.Resolve<ConnectionStrings>();
                return new ApplicationLoggerSettings
                {
                    ApplicationLoggerOutputSettingsCollection = new List<IApplicationLoggerOutputSettings>()
                    {
                        new MsSqlServerApplicationLoggerOutputSettings()
                        {
                            MinimumLogLevel = LogLevel.Verbose,
                            ConnectionString = connectionStrings.AppLogs
                        },
                    },
                    MinimumLogLevel = LogLevel.Verbose,
                    JobId = "Funding claim web job",
                    TaskKey = "Funding claim web job task",
                };
            }).As<IApplicationLoggerSettings>().SingleInstance();

            builder.RegisterType<ExecutionContext>().As<IExecutionContext>().InstancePerLifetimeScope();
            builder.RegisterType<SerilogLoggerFactory>().As<ISerilogLoggerFactory>().InstancePerLifetimeScope();
            builder.RegisterType<SeriLogger>().As<ILogger>().InstancePerLifetimeScope();
        }
    }
}

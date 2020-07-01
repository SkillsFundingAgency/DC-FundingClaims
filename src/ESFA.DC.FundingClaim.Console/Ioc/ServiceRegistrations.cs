using System;
using System.Collections.Generic;
using Autofac;
using ESFA.DC.FundingClaim.Console.Interfaces;
using ESFA.DC.FundingClaim.Console.Settings;
using ESFA.DC.FundingClaims.Data;
using ESFA.DC.FundingClaims.Message;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Queueing;
using ESFA.DC.Queueing.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.FundingClaim.Console.Ioc
{
    public class ServiceRegistrations : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FundingClaimsWebJob>().As<IFundingClaimsWebJob<MessageFundingClaimsSubmission>>().InstancePerLifetimeScope();

            builder.RegisterType<MessageFundingClaimsSubmission>().As<MessageFundingClaimsSubmission>().InstancePerLifetimeScope();

            builder.RegisterType<JsonSerializationService>().As<IJsonSerializationService>().InstancePerLifetimeScope();

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

            builder.Register(c =>
                    {
                        var config = c.Resolve<FundingClaimsQueueConfiguration>();
                        return new QueueSubscriptionService<MessageFundingClaimsSubmission>(
                        config,
                        c.Resolve<IJsonSerializationService>(),
                        c.Resolve<ILogger>());
                    })
                .As<IQueueSubscriptionService<MessageFundingClaimsSubmission>>();
        }
    }
}

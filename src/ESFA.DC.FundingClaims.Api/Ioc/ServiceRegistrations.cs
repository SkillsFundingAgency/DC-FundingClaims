using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.Api.Common.Settings;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.FuncingClaims.Services.Interfaces;
using ESFA.DC.FundingClaims.Data;
using ESFA.DC.FundingClaims.Message;
using ESFA.DC.FundingClaims.Model;
using ESFA.DC.FundingClaims.Model.Interfaces;
using ESFA.DC.FundingClaims.ReferenceData.Services;
using ESFA.DC.FundingClaims.ReferenceData.Services.Interfaces;
using ESFA.DC.FundingClaims.Services;
using ESFA.DC.ILR1819.DataStore.EF;
using ESFA.DC.ILR1819.DataStore.EF.Interface;
using ESFA.DC.ILR1920.DataStore.EF;
using ESFA.DC.ILR1920.DataStore.EF.Interface;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.Queueing;
using ESFA.DC.Queueing.Interface;
using ESFA.DC.ReferenceData.FCS.Model;
using ESFA.DC.ReferenceData.FCS.Model.Interface;
using ESFA.DC.ReferenceData.Organisations.Model;
using ESFA.DC.ReferenceData.Organisations.Model.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Summarisation.Model;
using ESFA.DC.Summarisation.Model.Interface;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.FundingClaims.Api.Ioc
{
    public class ServiceRegistrations : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FundingClaimsReferenceDataService>().As<IFundingClaimsReferenceDataService>().InstancePerLifetimeScope();
            builder.RegisterType<FundingClaimsService>().As<IFundingClaimsService>().InstancePerLifetimeScope();
            builder.RegisterType<FundingClaimsMessagingService>().As<IFundingClaimsMessagingService>().InstancePerLifetimeScope();
            builder.RegisterType<QueuePublishService<MessageFundingClaimsSubmission>>().As<IQueuePublishService<MessageFundingClaimsSubmission>>().InstancePerLifetimeScope();
            builder.RegisterType<FundingClaimsDataContext>().As<IFundingClaimsDataContext>().ExternallyOwned();
            builder.RegisterType<DateTimeProvider.DateTimeProvider>().As<IDateTimeProvider>().InstancePerLifetimeScope();
            builder.RegisterType<JsonSerializationService>().As<ISerializationService>().InstancePerLifetimeScope();

            builder.RegisterType<SummarisationContext>().As<ISummarisationContext>().ExternallyOwned();
            builder.RegisterType<JobQueueDataContext>().As<IJobQueueDataContext>().ExternallyOwned();
            builder.RegisterType<FcsContext>().As<IFcsContext>().ExternallyOwned();
            builder.RegisterType<ILR1819_DataStoreEntities>().As<IIlr1819RulebaseContext>().ExternallyOwned();
            builder.RegisterType<ILR1920_DataStoreEntities>().As<IIlr1920RulebaseContext>().ExternallyOwned();

            builder.RegisterType<OrganisationsContext>().As<IOrganisationsContext>().ExternallyOwned();

            builder.Register(context =>
                {
                    var connectionStrings = context.Resolve<ConnectionStrings>();
                    var optionsBuilder = new DbContextOptionsBuilder<JobQueueDataContext>();
                    optionsBuilder.UseSqlServer(
                        connectionStrings.KeyValues["JobManagement"],
                        options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));

                    return optionsBuilder.Options;
                })
                .As<DbContextOptions<JobQueueDataContext>>()
                .SingleInstance();

            builder.RegisterType<FundingStreamPeriodCodes1819>().Keyed<IFundingStreamPeriodCodes>(1819).InstancePerLifetimeScope();
            builder.RegisterType<FundingStreamPeriodCodes1920>().Keyed<IFundingStreamPeriodCodes>(1920).InstancePerLifetimeScope();

            builder.Register(context =>
            {
                var connectionStrings = context.Resolve<ConnectionStrings>();
                var optionsBuilder = new DbContextOptionsBuilder<FundingClaimsDataContext>();
                optionsBuilder.UseSqlServer(
                    connectionStrings.KeyValues["FundingClaims"],
                    options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));

                return optionsBuilder.Options;
            })
                .As<DbContextOptions<FundingClaimsDataContext>>()
                .SingleInstance();

            builder.Register(context =>
            {
                var connectionStrings = context.Resolve<ConnectionStrings>();
                var optionsBuilder = new DbContextOptionsBuilder<SummarisationContext>();
                optionsBuilder.UseSqlServer(
                    connectionStrings.KeyValues["SummarisedActuals"],
                    options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));

                return optionsBuilder.Options;
            })
                .As<DbContextOptions<SummarisationContext>>()
                .SingleInstance();

            builder.Register(context =>
                {
                    var connectionStrings = context.Resolve<ConnectionStrings>();
                    var optionsBuilder = new DbContextOptionsBuilder<FcsContext>();
                    optionsBuilder.UseSqlServer(
                        connectionStrings.KeyValues["FCS"],
                        options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));

                    return optionsBuilder.Options;
                })
                .As<DbContextOptions<FcsContext>>()
                .SingleInstance();

            builder.Register(context =>
                {
                    var connectionStrings = context.Resolve<ConnectionStrings>();
                    var optionsBuilder = new DbContextOptionsBuilder<OrganisationsContext>();
                    optionsBuilder.UseSqlServer(
                        connectionStrings.KeyValues["Org"],
                        options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));

                    return optionsBuilder.Options;
                })
                .As<DbContextOptions<OrganisationsContext>>()
                .SingleInstance();

            builder.Register(context =>
                {
                    var connectionStrings = context.Resolve<ConnectionStrings>();
                    var optionsBuilder = new DbContextOptionsBuilder<ILR1819_DataStoreEntities>();
                    optionsBuilder.UseSqlServer(
                        connectionStrings.KeyValues["ILR1819DataStore"],
                        options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));

                    return optionsBuilder.Options;
                })
                .As<DbContextOptions<ILR1819_DataStoreEntities>>()
                .SingleInstance();

            builder.Register(context =>
                {
                    var connectionStrings = context.Resolve<ConnectionStrings>();
                    var optionsBuilder = new DbContextOptionsBuilder<ILR1920_DataStoreEntities>();
                    optionsBuilder.UseSqlServer(
                        connectionStrings.KeyValues["ILR1920DataStore"],
                        options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));
                    return optionsBuilder.Options;
                })
                .As<DbContextOptions<ILR1920_DataStoreEntities>>()
                .SingleInstance();
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using Autofac;
using ESFA.DC.FundingClaims.AtomFeed.Services;
using ESFA.DC.FundingClaims.AtomFeed.Services.Interfaces;
using ESFA.DC.FundingClaims.Data;
using ESFA.DC.FundingClaims.Data.Entities;
using ESFA.DC.FundingClaims.Signing.Models;
using ESFA.DC.FundingClaims.Signing.Noticifications.Console.Configuration;
using ESFA.DC.FunidngClaims.Signing.Services;
using ESFA.DC.FunidngClaims.Signing.Services.Interfaces;
using ESFA.DC.Logging;
using ESFA.DC.Logging.Config;
using ESFA.DC.Logging.Config.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.ReferenceData.FCS.Service;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Xml;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using IHttpClientFactory = System.Net.Http.IHttpClientFactory;
using LogLevel = ESFA.DC.Logging.Enums.LogLevel;

namespace ESFA.DC.FundingClaims.Signing.Noticifications.Console.Ioc
{
    public class ServiceRegistrations : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FundingClaimsDataContext>().As<IFundingClaimsDataContext>().ExternallyOwned();
            builder.RegisterType<FeedDataStorageService>().As<IFeedDataStorageService>();
            builder.RegisterType<FeedItemMappingService>().As<IFeedItemMappingService>();
            builder.RegisterType<FundingClaimsFeedService>().As<IFundingClaimsFeedService>();
            builder.RegisterType<SyndicationFeedParserService<FundingClaimsFeedItem>>()
                .As<ISyndicationFeedParserService<FundingClaimsFeedItem>>();
            builder.RegisterType<SyndicationFeedService>().As<ISyndicationFeedService>();
            builder.RegisterType<XmlSerializationService>().As<IXmlSerializationService>().InstancePerLifetimeScope();

            //            builder.RegisterType<IHttpClientFactory>();
            //builder.RegisterType<DateTimeProvider.DateTimeProvider>().As<IDateTimeProvider>().InstancePerLifetimeScope();

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
                    TaskKey = "Funding Claims Signing notifications",
                    JobId = "Funding Claims Signing",
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
        }
    }
}

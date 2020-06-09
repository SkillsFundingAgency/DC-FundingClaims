using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.Api.Common.Settings.Extensions;
using ESFA.DC.FundingClaims.Api.Settings;
using ESFA.DC.Queueing.Interface.Configuration;
using Microsoft.Extensions.Configuration;

namespace ESFA.DC.FundingClaims.Api.Ioc
{
    public static class ConfigurationRegistration
    {
        public static void SetupServiceBusConfigurations(this ContainerBuilder builder, IConfiguration configuration)
        {
            builder.Register(c => configuration.GetConfigSection<FundingClaimsQueueConfiguration>())
                .As<IQueueConfiguration>().SingleInstance();
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.Api.Common.Ioc.Modules;
using ESFA.DC.FundingClaims.Api.Ioc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StartupBase = ESFA.DC.Api.Common.StartupBase;

namespace ESFA.DC.FundingClaims.Api
{
    public class Startup : StartupBase
    {
        public Startup(IWebHostEnvironment env)
            : base(env)
        {
        }

        public override void ConfigureContainer(ContainerBuilder containerBuilder)
        {
            containerBuilder.SetupConfigurations(Configuration);
            containerBuilder.SetupServiceBusConfigurations(Configuration);
            containerBuilder.RegisterModule<ServiceRegistrations>();
            containerBuilder.RegisterModule<LoggerRegistrations>();
        }
    }
}

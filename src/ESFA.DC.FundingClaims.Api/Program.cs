using System;
using System.Collections.Generic;
using System.IO;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ESFA.DC.FundingClaims.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args);

                host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
                host.ConfigureWebHostDefaults(webHostBuilder => {
                    webHostBuilder
                        .UseContentRoot(Directory.GetCurrentDirectory())
                        .UseIISIntegration()
                        .UseStartup<Startup>();
                });

            var hostRunner =  host.Build();

            hostRunner.Run();
        }
    }
}

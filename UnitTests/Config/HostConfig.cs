using Microservice.Core.Contexts;

namespace $safeprojectname$.Config
{
    using System.IO;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microservice.Core.Contracts;
    using Microservice.Core.Models.Config;
    using Microservice.Core.Services;

    public static class HostConfig
    {
        public static IHost Host { get; }

        static HostConfig()
        {
            Host = GetHostBuilder().Build();
        }

        private static IHostBuilder GetHostBuilder()
        {
            return new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false)
                        .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName.ToLower()}.json", optional: true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddOptions()
                        .Configure<DatabaseConfig>(hostContext.Configuration.GetSection("Database"))
                        .AddEntityFrameworkNpgsql()
                        .AddDbContextPool<DomainContext>(options =>
                        {
                            options.UseNpgsql(hostContext.Configuration.Get<DatabaseConfig>().ConnectionString);
                            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                        })
                        .AddSingleton<IMicroserviceService, MicroserviceService>();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging
                        .AddConfiguration(hostingContext.Configuration.GetSection("Logging"))
                        .AddConsole();
                });
        }
    }
}

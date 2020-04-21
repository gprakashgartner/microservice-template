using GdmTech.Messaging;
using GdmTech.Messaging.Contracts;

namespace $safeprojectname$
{
    using System.Threading.Tasks;
    using System.IO;
    using Core.Contexts;
    using Core.Contracts;
    using Core.Models.Config;
    using Core.Services;
    using GdmTech.Utils.Metrics;
    using GdmTech.Utils.Vault.Extensions;
    using Microsoft.Extensions.Options;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    class Program
    {
        private const string ServiceName = "Microservice";

        public static async Task Main(string[] args)
        {
            var builder = GetDaemonBuilder(args);
            await builder.RunConsoleAsync();
        }

        private static IHostBuilder GetDaemonBuilder(string[] args)
        {
            return new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false)
                        .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName.ToLower()}.json", optional: true)
                        .AddEnvironmentVariables()
                        .AddEnvironmentVariables($"{ServiceName}:")
                        .AddVault();

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddOptions()
                        .Configure<DatabaseConfig>(hostContext.Configuration.GetSection("Database"))
                        .Configure<MetricsOptions>(hostContext.Configuration.GetSection("Metrics"))
                        .Configure<EventBusOptions>(hostContext.Configuration.GetSection("Messaging"))
                        .Configure<MicroserviceConfig>(hostContext.Configuration.GetSection(ServiceName))
                        .AddSingleton<IHostedService, DaemonHostedService>()
                        .AddSingleton<MetricsCollectionFactory>()
                        .AddSingleton(provider => provider.GetService<MetricsCollectionFactory>().Create())
                        .AddSingleton<IEventBus, EventBus>()
                        .AddEntityFrameworkNpgsql()
                        .AddDbContext<DomainContext>((provider, options) =>
                        {
                            options.UseNpgsql(provider.GetService<IOptions<DatabaseConfig>>().Value.ConnectionString);
                            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                        }, ServiceLifetime.Transient)
                        .AddDbContext<DomainReadUncommittedContext>((provider, options) =>
                        {
                            options.UseNpgsql(provider.GetService<IOptions<DatabaseConfig>>().Value.ConnectionString);
                            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                        }, ServiceLifetime.Transient)
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

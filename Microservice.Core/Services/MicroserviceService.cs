namespace $safeprojectname$.Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GdmTech.Utils.Metrics;
    using Microsoft.Extensions.Logging;
    using Contracts;

    public class MicroserviceService : IMicroserviceService, IDaemonService
    {
        private readonly ILogger<MicroserviceService> logger;
        private readonly IServiceProvider serviceProvider;
        private readonly IMetricsCollectionService metricsCollection;

        public MicroserviceService(ILogger<MicroserviceService> logger,
            IServiceProvider serviceProvider,
            IMetricsCollectionService metricsCollection)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this.metricsCollection = metricsCollection ?? throw new ArgumentNullException(nameof(metricsCollection));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting...");

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Stopping...");

            return Task.FromException(new NotImplementedException());
        }
    }
}

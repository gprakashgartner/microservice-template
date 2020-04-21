namespace $safeprojectname$
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Core.Contracts;

    public class DaemonHostedService : IHostedService, IDisposable
    {
        private readonly IHostApplicationLifetime applicationLifetime;
        private readonly IDaemonService daemonService;
        private readonly ILogger logger;
        private readonly string daemonName;

        public DaemonHostedService(ILogger<DaemonHostedService> logger, IDaemonService daemonService, IHostApplicationLifetime applicationLifetime, string daemonName = null)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.daemonService = daemonService ?? throw new ArgumentNullException(nameof(daemonService));
            this.applicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
            this.daemonName = daemonName;
        }

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation($"Starting daemon: {daemonName}");

            // Register lifetime events
            applicationLifetime.ApplicationStarted.Register(OnStarted);
            applicationLifetime.ApplicationStopping.Register(OnStopping);
            applicationLifetime.ApplicationStopped.Register(OnStopped);

            return daemonService.StartAsync(cancellationToken);
        }

        public virtual Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation($"Stopping daemon: {daemonName}");

            return daemonService.StopAsync(cancellationToken);
        }

        protected virtual void OnStopped()
        {
            logger.LogInformation("Daemon stopped.");
        }

        protected virtual void OnStopping()
        {
            logger.LogInformation("Stopping daemon.");
        }

        protected virtual void OnStarted()
        {
            logger.LogInformation("Daemon started.");
        }

        public void Dispose()
        {
            logger.LogInformation("Disposing....");
        }
    }
}

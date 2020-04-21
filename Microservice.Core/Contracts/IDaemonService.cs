namespace $safeprojectname$.Contracts
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IDaemonService
    {
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
    }
}

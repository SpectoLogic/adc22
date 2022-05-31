namespace adc22config.Services
{
    public interface IConfigurationUpdater
    {
        Task StartMonitoring(CancellationToken cancellationToken);
        Task StopMonitoring(CancellationToken cancellationToken);
    }
}
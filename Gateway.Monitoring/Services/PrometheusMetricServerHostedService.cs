using Gateway.Monitoring.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Prometheus;

namespace Gateway.Monitoring.Services;

public class PrometheusMetricServerHostedService(
    ILogger<PrometheusMetricServerHostedService> logger,
    IOptions<PrometheusMonitoringOptions> options
) : IHostedService
{
    private KestrelMetricServer? _server;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _server = new KestrelMetricServer(
            hostname: options.Value.KestrelMetricServerHost,
            port: options.Value.KestrelMetricServerPort
        );
        try
        {
            _server.Start();
            logger.LogInformation(message: "Prometheus metrics server started on port {KestrelMetricServerPort}",
                options.Value.KestrelMetricServerPort);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                message: "Could not start Metrics Server. Check if port {KestrelMetricServerPort} is in use.",
                options.Value.KestrelMetricServerPort);
        }

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_server == null) return;

        try
        {
            logger.LogInformation(message: "Stopping Prometheus metrics server...");
            await _server.StopAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error occurred while stopping Prometheus metrics server gracefully");
        }
        finally
        {
            _server = null;
        }
    }
}
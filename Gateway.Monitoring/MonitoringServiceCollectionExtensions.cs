using Gateway.Monitoring.Configuration;
using Gateway.Monitoring.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Gateway.Monitoring;

public static class MonitoringServiceCollectionExtensions
{
    public static void UseMonitoring(this HostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IMetricsService, PrometheusMetricsService>();

        builder.Services.Configure<PrometheusMonitoringOptions>(
            builder.Configuration.GetSection("PrometheusMonitoringOptions"));

        builder.Services.AddHostedService<PrometheusMetricServerHostedService>();
    }
}
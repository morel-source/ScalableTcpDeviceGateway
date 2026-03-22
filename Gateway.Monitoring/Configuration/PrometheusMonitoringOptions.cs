namespace Gateway.Monitoring.Configuration;

public record PrometheusMonitoringOptions(
    int KestrelMetricServerPort)
{
    public PrometheusMonitoringOptions() : this(
        KestrelMetricServerPort: 1234)
    {
    }
}
namespace Gateway.Monitoring.Configuration;

public record PrometheusMonitoringOptions(
    int KestrelMetricServerPort,
    string KestrelMetricServerHost)
{
    public PrometheusMonitoringOptions() : this(
        KestrelMetricServerPort: 1234,
        KestrelMetricServerHost: "0.0.0.0")
    {
    }
}
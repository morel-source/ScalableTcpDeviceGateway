using Prometheus;

namespace Gateway.Monitoring.Metrics;

public static class MetricsRegistry
{
    public static readonly Gauge TotalNumberDevices =
        Prometheus.Metrics.CreateGauge(
            name: "gateway_devices_expected_total",
            help: "Planned device count"
        );

    public static readonly Gauge ActiveConnections =
        Prometheus.Metrics.CreateGauge(
            name: "gateway_active_connections",
            help: "Current concurrent TCP sessions"
        );

    public static readonly Counter TotalLoginMessages =
        Prometheus.Metrics.CreateCounter(
            name: "gateway_logins_total",
            help: "Total successful login events",
            new CounterConfiguration
            {
                LabelNames = ["instance_id"],
            }
        );

    public static readonly Counter TotalHeartBeatMessages =
        Prometheus.Metrics.CreateCounter(
            name: "gateway_heartbeats_total",
            help: "Total heartbeats processed",
            new CounterConfiguration
            {
                LabelNames = ["instance_id"]
            }
        );

    public static readonly Counter TotalDisconnectMessages =
        Prometheus.Metrics.CreateCounter(
            name: "gateway_disconnects_total",
            help: "Total device disconnections",
            new CounterConfiguration
            {
                LabelNames = ["instance_id"]
            }
        );

    public static readonly Histogram LoginProcessingDuration =
        Prometheus.Metrics.CreateHistogram(
            name: "gateway_login_duration_seconds",
            help: "Histogram of login processing latency",
            new HistogramConfiguration
            {
                LabelNames = ["instance_id"],
                Buckets = [0.01, 0.05, 0.1, 0.5, 1.0, 2.0, 5.0, 10.0]
            });

    public static readonly Histogram HeartbeatProcessingDuration =
        Prometheus.Metrics.CreateHistogram(
            name: "gateway_heartbeat_duration_seconds",
            help: "Histogram of heartbeat processing latency",
            new HistogramConfiguration
            {
                LabelNames = ["instance_id"],
                Buckets = [0.01, 0.05, 0.1, 0.5, 1.0, 2.0, 5.0, 10.0]
            }
        );
}
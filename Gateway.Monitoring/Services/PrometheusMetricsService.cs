using Gateway.Monitoring.Metrics;
using Prometheus;

namespace Gateway.Monitoring.Services;

public class PrometheusMetricsService : IMetricsService
{
    public void IncrementActiveConnections() =>
        MetricsRegistry.ActiveConnections.Inc();

    public void DecrementActiveConnections() =>
        MetricsRegistry.ActiveConnections.Dec();

    public void ResetActiveConnections() =>
        MetricsRegistry.ActiveConnections.Set(val: 0);

    public void SetExpectedDevices(int deviceCount) =>
        MetricsRegistry.TotalNumberDevices.Set(val: deviceCount);

    public void IncrementLoginConnections() =>
        MetricsRegistry.TotalLoginMessages.WithLabels(Environment.MachineName).Inc();

    public void IncrementHeartBeatConnections() =>
        MetricsRegistry.TotalHeartBeatMessages.WithLabels(Environment.MachineName).Inc();

    public void IncrementDisconnectConnections() =>
        MetricsRegistry.TotalDisconnectMessages.WithLabels(Environment.MachineName).Inc();

    public IDisposable MeasureLoginProcess() =>
        MetricsRegistry.LoginProcessingDuration.WithLabels(Environment.MachineName).NewTimer();

    public IDisposable MeasureHeartBeatProcess() =>
        MetricsRegistry.HeartbeatProcessingDuration.WithLabels(Environment.MachineName).NewTimer();

    public void IncrementTelemetryCount() =>
        MetricsRegistry.TotalTelemetryMessages.WithLabels(Environment.MachineName).Inc();

    public void IncrementAlertCount() =>
        MetricsRegistry.TotalAlertMessages.WithLabels(Environment.MachineName).Inc();

    public void IncrementDeadLetterCount() =>
        MetricsRegistry.TotalDeadLetterMessages.WithLabels(Environment.MachineName).Inc();

    public IDisposable MeasureTelemetryProcess() =>
        MetricsRegistry.TelemetryProcessingDuration.WithLabels(Environment.MachineName).NewTimer();

    public void RecordTemperature(string deviceId, double celsius) =>
        MetricsRegistry.TemperatureByDevice
            .WithLabels(deviceId, Environment.MachineName)
            .Set(celsius);
}
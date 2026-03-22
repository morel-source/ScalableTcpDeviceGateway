using Gateway.Monitoring.Metrics;
using Prometheus;

namespace Gateway.Monitoring.Services;

public class PrometheusMetricsService : IMetricsService
{
    public void IncrementActiveConnections()
    {
        MetricsRegistry.ActiveConnections.Inc();
    }

    public void DecrementActiveConnections()
    {
        MetricsRegistry.ActiveConnections.Dec();
    }

    public void ResetActiveConnections()
    {
        MetricsRegistry.ActiveConnections.Set(val: 0);
    }

    public void SetExpectedDevices(int deviceCount)
    {
        MetricsRegistry.TotalNumberDevices.Set(val: deviceCount);
    }

    public void IncrementLoginConnections()
    {
        MetricsRegistry.TotalLoginMessages.WithLabels(Environment.MachineName).Inc();
    }

    public void IncrementHeartBeatConnections()
    {
        MetricsRegistry.TotalHeartBeatMessages.WithLabels(Environment.MachineName).Inc();
    }

    public void IncrementDisconnectConnections()
    {
        MetricsRegistry.TotalDisconnectMessages.WithLabels(Environment.MachineName).Inc();
    }

    public IDisposable MeasureLoginProcess()
    {
        return MetricsRegistry.LoginProcessingDuration.WithLabels(Environment.MachineName).NewTimer();
    }

    public IDisposable MeasureHeartBeatProcess()
    {
        return MetricsRegistry.HeartbeatProcessingDuration.WithLabels(Environment.MachineName).NewTimer();
    }

    public IDisposable TrackConnection()
    {
        IncrementActiveConnections();
        // Return a disposable that automatically decrements when the 'using' block ends
        return new DisposableAction(DecrementActiveConnections);
    }
}

// Simple helper class
class DisposableAction(Action onDispose) : IDisposable
{
    public void Dispose() => onDispose();
}
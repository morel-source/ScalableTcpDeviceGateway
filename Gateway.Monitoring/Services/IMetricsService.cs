namespace Gateway.Monitoring.Services;

public interface IMetricsService
{
    void SetExpectedDevices(int deviceCount);
    void IncrementActiveConnections();
    void DecrementActiveConnections();
    void ResetActiveConnections();
    void IncrementLoginConnections();
    void IncrementHeartBeatConnections();
    void IncrementDisconnectConnections();
    IDisposable MeasureLoginProcess();
    IDisposable MeasureHeartBeatProcess();
    IDisposable TrackConnection();
}
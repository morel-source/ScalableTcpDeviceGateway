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
    void IncrementTelemetryCount();
    void IncrementAlertCount();
    void IncrementDeadLetterCount();
    IDisposable MeasureTelemetryProcess();
    void RecordTemperature(string deviceId, double celsius);
}
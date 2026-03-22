namespace Device.Simulator.Configuration;

public record SimulatorOptions(
    int DeviceCount,
    int ServerPort,
    string ServerHost,
    TimeSpan HeartbeatInterval,
    TimeSpan AckTimeout,
    int DeviceConnectionDelaySec,
    int ConcurrentConnection,
    bool SimulateDisconnections
)
{
    public SimulatorOptions() : this(
        DeviceCount: 1000,
        ServerPort: 8888,
        ServerHost: "127.0.0.1",
        HeartbeatInterval: TimeSpan.FromMinutes(5),
        AckTimeout: TimeSpan.FromSeconds(5),
        DeviceConnectionDelaySec: 100,
        ConcurrentConnection: 100,
        SimulateDisconnections: true
    )
    {
    }
}
namespace Gateway.Server.Configuration;

public record DeviceConnectionOptions(
    TimeSpan LoginTimeout,
    TimeSpan HeartbeatTimeout
)
{
    public DeviceConnectionOptions() : this(
        LoginTimeout: TimeSpan.FromSeconds(5),
        HeartbeatTimeout: TimeSpan.FromMinutes(1)
    )
    {
    }
}
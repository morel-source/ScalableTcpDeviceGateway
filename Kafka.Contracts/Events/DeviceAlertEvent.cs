using Gateway.Protocol.Enums;

namespace Kafka.Contracts.Events;

public readonly record struct DeviceAlertEvent
{
    public required string DeviceId { get; init; }
    public required AlertType AlertType { get; init; }
    public required string Message { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
}
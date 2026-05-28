using Gateway.Protocol.Enums;

namespace Kafka.Contracts.Events;

public readonly record struct DeviceStatusEvent
{
    public required string DeviceId { get; init; }
    public required DeviceStatusType Status { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
    public string? DisconnectReason { get; init; }
}
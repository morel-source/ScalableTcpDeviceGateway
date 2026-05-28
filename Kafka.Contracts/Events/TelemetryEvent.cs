namespace Kafka.Contracts.Events;

public readonly record struct TelemetryEvent
{
    public required string DeviceId { get; init; }
    public required double Temperature { get; init; }
    public required int BatteryLevel { get; init; }
    public required int SignalStrength { get; init; }
    public required DateTimeOffset ReceivedAt { get; init; }
    public required string GatewayInstance { get; init; }
}
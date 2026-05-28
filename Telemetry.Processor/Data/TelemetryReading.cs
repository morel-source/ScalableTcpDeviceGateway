namespace Telemetry.Processor.Data;

public sealed class TelemetryReading
{
    public long Id { get; set; }
    public required string DeviceId { get; set; }
    public double Temperature { get; set; }
    public int BatteryLevel { get; set; }
    public int SignalStrength { get; set; }
    public DateTimeOffset ReceivedAt { get; set; }
    public DateTimeOffset StoredAt { get; set; }
}
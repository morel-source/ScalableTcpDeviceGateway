using Gateway.Protocol.Enums;
using Gateway.Protocol.Payloads.Base;
using Gateway.Protocol.Payloads.Fields;

namespace Gateway.Protocol.Payloads.Messages;

public readonly record struct TelemetryMessagePayload(
    BarcodePayload DeviceBarcode,
    TemperaturePayload Temperature,
    BatteryPayload Battery,
    SignalPayload Signal,
    TimestampPayload Timestamp
) : IMessagePayload
{
    public TelemetryMessagePayload(string deviceBarcode, double temperature, byte battery, byte signal,
        DateTime timestamp)
        : this(
            new BarcodePayload(deviceBarcode),
            new TemperaturePayload(temperature),
            new BatteryPayload(battery),
            new SignalPayload(signal),
            new TimestampPayload(timestamp))
    {
    }

    public int FixedSize =>
        DeviceBarcode.FixedSize +
        Temperature.FixedSize +
        Battery.FixedSize +
        Signal.FixedSize +
        Timestamp.FixedSize;

    public MessageType MessageType => MessageType.Telemetry;
}
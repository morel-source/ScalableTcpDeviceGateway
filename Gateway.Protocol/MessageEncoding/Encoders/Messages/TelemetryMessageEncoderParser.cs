using Gateway.Protocol.MessageEncoding.Base;
using Gateway.Protocol.MessageEncoding.Encoders.Fields;
using Gateway.Protocol.Payloads.Messages;

namespace Gateway.Protocol.MessageEncoding.Encoders.Messages;

public sealed class TelemetryMessageEncoderParser(
    BarcodeEncoderParser barcodeEncoderParser,
    TemperatureEncoderParser temperatureEncoderParser,
    BatteryEncoderParser batteryEncoderParser,
    SignalEncoderParser signalEncoderParser,
    TimestampEncoderParser timestampEncoderParser
) : EncoderBase<TelemetryMessagePayload>
{
    protected override void Encode(ref Span<byte> buffer, TelemetryMessagePayload payload, ref int position)
    {
        barcodeEncoderParser.Encode(ref buffer, payload.DeviceBarcode, ref position);
        temperatureEncoderParser.Encode(ref buffer, payload.Temperature, ref position);
        batteryEncoderParser.Encode(ref buffer, payload.Battery, ref position);
        signalEncoderParser.Encode(ref buffer, payload.Signal, ref position);
        timestampEncoderParser.Encode(ref buffer, payload.Timestamp, ref position);
    }
}
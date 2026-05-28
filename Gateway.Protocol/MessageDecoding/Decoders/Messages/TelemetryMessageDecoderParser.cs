using System.Buffers;
using Gateway.Protocol.MessageDecoding.Base;
using Gateway.Protocol.MessageDecoding.Decoders.Fields;
using Gateway.Protocol.Payloads.Messages;

namespace Gateway.Protocol.MessageDecoding.Decoders.Messages;

public sealed class TelemetryMessageDecoderParser(
    BarcodeDecoderParser barcodeParser,
    TemperatureDecoderParser temperatureParser,
    BatteryDecoderParser batteryParser,
    SignalDecoderParser signalParser,
    TimestampDecoderParser timestampParser
) : DecoderBase<TelemetryMessagePayload>
{
    protected override Result<TelemetryMessagePayload> Decode(ref SequenceReader<byte> reader)
    {
        var barcode = barcodeParser.Decode(ref reader);
        if (!barcode.Ok)
            return Result<TelemetryMessagePayload>.Failure("Failed to decode barcode");

        var temperature = temperatureParser.Decode(ref reader);
        if (!temperature.Ok)
            return Result<TelemetryMessagePayload>.Failure("Failed to decode temperature");

        var battery = batteryParser.Decode(ref reader);
        if (!battery.Ok)
            return Result<TelemetryMessagePayload>.Failure("Failed to decode battery");

        var signal = signalParser.Decode(ref reader);
        if (!signal.Ok)
            return Result<TelemetryMessagePayload>.Failure("Failed to decode signal");

        var timestamp = timestampParser.Decode(ref reader);
        if (!timestamp.Ok)
            return Result<TelemetryMessagePayload>.Failure("Failed to decode timestamp");

        return Result<TelemetryMessagePayload>.Success(new TelemetryMessagePayload(
            barcode.Payload,
            temperature.Payload,
            battery.Payload,
            signal.Payload,
            timestamp.Payload));
    }
}
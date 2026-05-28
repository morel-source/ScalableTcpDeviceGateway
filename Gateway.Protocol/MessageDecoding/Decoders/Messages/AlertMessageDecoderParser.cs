using System.Buffers;
using Gateway.Protocol.MessageDecoding.Base;
using Gateway.Protocol.MessageDecoding.Decoders.Fields;
using Gateway.Protocol.Payloads.Messages;

namespace Gateway.Protocol.MessageDecoding.Decoders.Messages;

public sealed class AlertMessageDecoderParser(
    BarcodeDecoderParser barcodeParser,
    AlertTypeDecoderParser alertTypeParser,
    TimestampDecoderParser timestampParser
) : DecoderBase<AlertMessagePayload>
{
    protected override Result<AlertMessagePayload> Decode(ref SequenceReader<byte> reader)
    {
        var barcode = barcodeParser.Decode(ref reader);
        if (!barcode.Ok)
            return Result<AlertMessagePayload>.Failure("Failed to decode barcode");

        var alertType = alertTypeParser.Decode(ref reader);
        if (!alertType.Ok)
            return Result<AlertMessagePayload>.Failure("Failed to decode alert type");

        var timestamp = timestampParser.Decode(ref reader);
        if (!timestamp.Ok)
            return Result<AlertMessagePayload>.Failure("Failed to decode timestamp");

        return Result<AlertMessagePayload>.Success(
            new AlertMessagePayload(barcode.Payload, alertType.Payload, timestamp.Payload));
    }
}
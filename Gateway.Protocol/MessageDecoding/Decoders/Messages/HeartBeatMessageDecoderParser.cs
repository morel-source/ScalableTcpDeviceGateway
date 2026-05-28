using System.Buffers;
using Gateway.Protocol.MessageDecoding.Base;
using Gateway.Protocol.MessageDecoding.Decoders.Fields;
using Gateway.Protocol.Payloads.Messages;

namespace Gateway.Protocol.MessageDecoding.Decoders.Messages;

public sealed class HeartBeatMessageDecoderParser(
    BarcodeDecoderParser barcodeParser,
    TimestampDecoderParser timestampParser
) : DecoderBase<HeartbeatMessagePayload>
{
    protected override Result<HeartbeatMessagePayload> Decode(ref SequenceReader<byte> reader)
    {
        var barcode = barcodeParser.Decode(ref reader);
        if (!barcode.Ok)
            return Result<HeartbeatMessagePayload>.Failure("failed to decode barcode");

        var timestamp = timestampParser.Decode(ref reader);
        if (!timestamp.Ok)
            return Result<HeartbeatMessagePayload>.Failure("failed to decode timestamp");

        return Result<HeartbeatMessagePayload>.Success(new HeartbeatMessagePayload(barcode.Payload, timestamp.Payload));
    }
}
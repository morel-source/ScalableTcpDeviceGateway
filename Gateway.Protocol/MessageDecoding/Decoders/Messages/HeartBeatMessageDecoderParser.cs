using System.Buffers;
using Gateway.Protocol.MessageDecoding.Base;
using Gateway.Protocol.MessageDecoding.Decoders.Fields;
using Gateway.Protocol.Payloads;

namespace Gateway.Protocol.MessageDecoding.Decoders.Messages;

public sealed class HeartBeatMessageDecoderParser(
    BarcodeDecoderParser barcodeParser,
    TimestampDecoderParser timestampParser
) : DecoderBase<HeartbeatPayload>
{
    protected override Result<HeartbeatPayload> Decode(ref SequenceReader<byte> reader)
    {
        var barcode = barcodeParser.Decode(ref reader);
        if (!barcode.Ok)
            return Result<HeartbeatPayload>.Failure("failed to decode barcode");

        var timestamp = timestampParser.Decode(ref reader);
        if (!timestamp.Ok)
            return Result<HeartbeatPayload>.Failure("failed to decode timestamp");

        return Result<HeartbeatPayload>.Success(
            new HeartbeatPayload(barcode.Payload, timestamp.Payload));
    }
}
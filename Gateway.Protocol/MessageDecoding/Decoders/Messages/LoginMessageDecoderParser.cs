using System.Buffers;
using Gateway.Protocol.MessageDecoding.Base;
using Gateway.Protocol.MessageDecoding.Decoders.Fields;
using Gateway.Protocol.Payloads;

namespace Gateway.Protocol.MessageDecoding.Decoders.Messages;

public sealed class LoginMessageDecoderParser(
    BarcodeDecoderParser barcodeParser,
    TimestampDecoderParser timestampParser
) : DecoderBase<LoginPayload>
{
    protected override Result<LoginPayload> Decode(ref SequenceReader<byte> reader)
    {
        var barcode = barcodeParser.Decode(ref reader);
        if (!barcode.Ok)
            return Result<LoginPayload>.Failure("failed to decode barcode");

        var timestamp = timestampParser.Decode(ref reader);
        if (!timestamp.Ok)
            return Result<LoginPayload>.Failure("failed to decode timestamp");

        return Result<LoginPayload>.Success(new LoginPayload(
            barcode.Payload, timestamp.Payload));
    }
}
using System.Buffers;
using Gateway.Protocol.MessageDecoding.Base;
using Gateway.Protocol.MessageDecoding.Decoders.Fields;
using Gateway.Protocol.Payloads.Messages;

namespace Gateway.Protocol.MessageDecoding.Decoders.Messages;

public sealed class LoginMessageDecoderParser(
    BarcodeDecoderParser barcodeParser,
    TimestampDecoderParser timestampParser
) : DecoderBase<LoginMessagePayload>
{
    protected override Result<LoginMessagePayload> Decode(ref SequenceReader<byte> reader)
    {
        var barcode = barcodeParser.Decode(ref reader);
        if (!barcode.Ok)
            return Result<LoginMessagePayload>.Failure("failed to decode barcode");

        var timestamp = timestampParser.Decode(ref reader);
        if (!timestamp.Ok)
            return Result<LoginMessagePayload>.Failure("failed to decode timestamp");

        return Result<LoginMessagePayload>.Success(new LoginMessagePayload(
            barcode.Payload, timestamp.Payload));
    }
}
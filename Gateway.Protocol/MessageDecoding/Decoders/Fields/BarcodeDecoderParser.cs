using System.Buffers;
using Gateway.Protocol.MessageDecoding.Interfaces;
using Gateway.Protocol.Payloads;

namespace Gateway.Protocol.MessageDecoding.Decoders.Fields;

public sealed class BarcodeDecoderParser : IFieldDecoder<BarcodePayload>
{
    public Result<BarcodePayload> Decode(ref SequenceReader<byte> reader)
    {
        if (!(
                reader.TryRead(out byte firstByte) &&
                reader.TryRead(out byte secondByte) &&
                reader.TryRead(out byte thirdByte) &&
                reader.TryRead(out byte fourthByte) &&
                reader.TryRead(out byte fifthByte) &&
                reader.TryRead(out byte sixthByte)
            ))
            return Result<BarcodePayload>.Failure("Invalid messageType");

        return Result<BarcodePayload>.Success(new BarcodePayload(
            new ReadOnlyMemory<byte>([
                firstByte, secondByte, thirdByte, fourthByte, fifthByte, sixthByte
            ])
        ));
    }
}
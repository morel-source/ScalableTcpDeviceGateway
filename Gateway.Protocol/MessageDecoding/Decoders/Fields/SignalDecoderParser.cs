using System.Buffers;
using Gateway.Protocol.MessageDecoding.Interfaces;
using Gateway.Protocol.Payloads.Fields;

namespace Gateway.Protocol.MessageDecoding.Decoders.Fields;

public sealed class SignalDecoderParser : IFieldDecoder<SignalPayload>
{
    public Result<SignalPayload> Decode(ref SequenceReader<byte> reader)
    {
        if (!reader.TryRead(out byte strength))
            return Result<SignalPayload>.Failure("Failed to decode signal");

        return Result<SignalPayload>.Success(new SignalPayload(strength));
    }
}
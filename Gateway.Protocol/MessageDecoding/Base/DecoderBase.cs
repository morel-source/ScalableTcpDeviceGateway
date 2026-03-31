using System.Buffers;
using Gateway.Protocol.MessageDecoding.Interfaces;
using Gateway.Protocol.Payloads;

namespace Gateway.Protocol.MessageDecoding.Base;

public abstract class DecoderBase<TPayload> : IMessageDecoder<TPayload>
    where TPayload : IPayload
{
    IPayload IMessageDecoder.Decode(ReadOnlySequence<byte> buffer)
    {
        var result = Decode(buffer);
        if (!result.Ok)
            throw new Exception("failed to decode message");
        return result.Payload;
    }

    public Result<TPayload> Decode(ReadOnlySequence<byte> buffer)
    {
        var reader = new SequenceReader<byte>(buffer);
        return Decode(ref reader);
    }

    protected abstract Result<TPayload> Decode(ref SequenceReader<byte> reader);
}
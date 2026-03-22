using System.Buffers;
using Gateway.Protocol.MessageDecoding.Base.Interfaces;
using Gateway.Protocol.Payloads;

namespace Gateway.Protocol.MessageDecoding.Base;

public abstract class DecoderBase<TPayload> : IMessageDecoder<TPayload>
    where TPayload : IPayload
{
    IPayload IMessageDecoder.Decode(ref SequenceReader<byte> reader) => Decode(ref reader);

    public abstract TPayload Decode(ref SequenceReader<byte> reader);
}
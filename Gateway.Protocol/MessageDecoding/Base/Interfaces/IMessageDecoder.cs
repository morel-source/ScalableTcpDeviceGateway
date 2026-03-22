using System.Buffers;
using Gateway.Protocol.Payloads;

namespace Gateway.Protocol.MessageDecoding.Base.Interfaces;

public interface IMessageDecoder
{
    IPayload Decode(ref SequenceReader<byte> buffer);
}

public interface IMessageDecoder<out TPayload> : IMessageDecoder
    where TPayload : IPayload
{
    new TPayload Decode(ref SequenceReader<byte> buffer);
}
using System.Buffers;
using Gateway.Protocol.Payloads;

namespace Gateway.Protocol.MessageDecoding.Interfaces;

public interface IMessageDecoder
{
    IPayload Decode(ReadOnlySequence<byte> buffer);
}

public interface IMessageDecoder<TPayload> : IMessageDecoder
{
    new Result<TPayload> Decode(ReadOnlySequence<byte> buffer);
}
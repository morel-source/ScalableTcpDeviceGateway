using System.Buffers;

namespace Gateway.Protocol.MessageDecoding.Interfaces;

public interface IFieldDecoder<TPayload>
{
    Result<TPayload> Decode(ref SequenceReader<byte> reader);
}
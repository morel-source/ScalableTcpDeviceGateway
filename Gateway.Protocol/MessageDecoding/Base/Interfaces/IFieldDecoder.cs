using System.Buffers;

namespace Gateway.Protocol.MessageDecoding.Base.Interfaces;

public interface IFieldDecoder<out TPayload>
{
    TPayload Decode(ref SequenceReader<byte> buffer);
}

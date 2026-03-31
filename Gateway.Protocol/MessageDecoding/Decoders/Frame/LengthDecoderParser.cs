using System.Buffers;
using Gateway.Protocol.MessageDecoding.Interfaces;

namespace Gateway.Protocol.MessageDecoding.Decoders.Frame;

public class LengthDecoderParser : IFieldDecoder<int>
{
    public Result<int> Decode(ref SequenceReader<byte> buffer)
    {
        if (!buffer.TryRead(out byte length))
        {
            return Result<int>.Failure("Missing length byte");
        }
        return Result<int>.Success(length);
    }
}
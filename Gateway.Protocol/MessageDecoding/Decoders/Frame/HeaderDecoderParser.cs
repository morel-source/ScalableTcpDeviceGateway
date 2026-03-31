using System.Buffers;
using Gateway.Protocol.Enums;
using Gateway.Protocol.MessageDecoding.Interfaces;

namespace Gateway.Protocol.MessageDecoding.Decoders.Frame;

public sealed class HeaderDecoderParser : IFieldDecoder<FrameByte>
{
    public Result<FrameByte> Decode(ref SequenceReader<byte> buffer)
    {
        if (!buffer.TryRead(out byte startByte))
            return Result<FrameByte>.Failure("Missing start byte");

        if (!Enum.IsDefined(typeof(FrameByte), startByte))
            return Result<FrameByte>.Failure("Invalid start byte");

        return Result<FrameByte>.Success((FrameByte)startByte);
    }
}
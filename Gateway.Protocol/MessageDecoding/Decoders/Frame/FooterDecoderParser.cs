using System.Buffers;
using Gateway.Protocol.Enums;
using Gateway.Protocol.MessageDecoding.Interfaces;

namespace Gateway.Protocol.MessageDecoding.Decoders.Frame;

public sealed class FooterDecoderParser : IFieldDecoder<FrameByte>
{
    public Result<FrameByte> Decode(ref SequenceReader<byte> buffer)
    {
        if (!buffer.TryRead(out byte endByte))
            return Result<FrameByte>.Failure("Missing end byte");

        if (!Enum.IsDefined(typeof(FrameByte), endByte))
            return Result<FrameByte>.Failure("Invalid end byte");

        return Result<FrameByte>.Success((FrameByte)endByte);
    }
}
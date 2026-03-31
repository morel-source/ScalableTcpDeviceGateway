using Gateway.Protocol.Enums;
using Gateway.Protocol.MessageEncoding.Interfaces;

namespace Gateway.Protocol.MessageEncoding.Encoders.Frame;

public class FooterEncoderParser : IFieldEncoder<FrameByte>
{
    public void Encode(ref Span<byte> buffer, FrameByte field, ref int position)
    {
        buffer[position++] = (byte)field;
    }
}
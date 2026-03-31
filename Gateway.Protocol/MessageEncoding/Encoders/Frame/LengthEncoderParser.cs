using Gateway.Protocol.MessageEncoding.Interfaces;

namespace Gateway.Protocol.MessageEncoding.Encoders.Frame;

public class LengthEncoderParser : IFieldEncoder<int>
{
    public void Encode(ref Span<byte> buffer, int field, ref int position)
    {
        if (field > 0xFF)
            throw new InvalidCastException("Fail to encode, length is invalid");

        buffer[position++] = (byte)field;
    }
}
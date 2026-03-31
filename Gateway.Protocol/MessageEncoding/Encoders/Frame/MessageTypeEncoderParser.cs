using Gateway.Protocol.Enums;
using Gateway.Protocol.MessageEncoding.Interfaces;

namespace Gateway.Protocol.MessageEncoding.Encoders.Frame;

public class MessageTypeEncoderParser : IFieldEncoder<MessageType>
{
    public void Encode(ref Span<byte> buffer, MessageType field, ref int position)
    {
        buffer[position++] = (byte)field;
    }
}
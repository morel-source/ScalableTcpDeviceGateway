using Gateway.Protocol.Enums;
using Gateway.Protocol.MessageEncoding.Base.Interfaces;
using Gateway.Protocol.Payloads;

namespace Gateway.Protocol.MessageEncoding.Encoders.Messages;

public sealed class AckMessageEncoderParser : IMessageEncoder<AckPayload>
{
    public byte[] Encode(AckPayload payload)
    {
        var buffer = new byte[AckPayload.FixedSize];
        var position = 0;

        buffer[position++] = (byte)MessageType.StartByte;
        buffer[position++] = (byte)MessageType.Ack;

        buffer[position] = (byte)MessageType.EndByte;

        return buffer;
    }
}
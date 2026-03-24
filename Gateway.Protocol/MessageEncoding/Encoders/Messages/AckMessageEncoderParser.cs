using Gateway.Protocol.Enums;
using Gateway.Protocol.MessageEncoding.Base;
using Gateway.Protocol.Payloads;

namespace Gateway.Protocol.MessageEncoding.Encoders.Messages;

public sealed class AckMessageEncoderParser : EncoderBase<AckPayload>
{
    protected override void Encode(ref Span<byte> buffer, AckPayload payload, ref int position)
    {
        buffer[position++] = (byte)MessageType.StartByte;
        buffer[position++] = (byte)MessageType.Ack;
        buffer[position++] = (byte)MessageType.EndByte;
    }
}
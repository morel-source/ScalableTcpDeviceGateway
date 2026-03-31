using Gateway.Protocol.MessageEncoding.Base;
using Gateway.Protocol.Payloads;

namespace Gateway.Protocol.MessageEncoding.Encoders.Messages;

public sealed class AckMessageEncoderParser : EncoderBase<AckPayload>
{
    protected override void Encode(ref Span<byte> buffer, AckPayload payload, ref int position)
    {
        buffer[position++] = (byte)payload.MessageTypeAck;
    }
}
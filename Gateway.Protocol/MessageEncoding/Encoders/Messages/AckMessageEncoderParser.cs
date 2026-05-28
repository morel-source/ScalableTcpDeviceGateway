using Gateway.Protocol.MessageEncoding.Base;
using Gateway.Protocol.Payloads.Messages;

namespace Gateway.Protocol.MessageEncoding.Encoders.Messages;

public sealed class AckMessageEncoderParser : EncoderBase<AckMessagePayload>
{
    protected override void Encode(ref Span<byte> buffer, AckMessagePayload payload, ref int position)
    {
        buffer[position++] = (byte)payload.MessageTypeAck;
    }
}
using Gateway.Protocol.MessageEncoding.Interfaces;
using Gateway.Protocol.Payloads.Fields;

namespace Gateway.Protocol.MessageEncoding.Encoders.Fields;

public sealed class AlertTypeEncoderParser : IFieldEncoder<AlertTypePayload>
{
    public void Encode(ref Span<byte> buffer, AlertTypePayload payload, ref int position)
    {
        buffer[position++] = (byte)payload.AlertType;
    }
}
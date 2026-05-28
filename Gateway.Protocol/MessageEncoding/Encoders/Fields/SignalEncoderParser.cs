using Gateway.Protocol.MessageEncoding.Interfaces;
using Gateway.Protocol.Payloads.Fields;

namespace Gateway.Protocol.MessageEncoding.Encoders.Fields;

public sealed class SignalEncoderParser : IFieldEncoder<SignalPayload>
{
    public void Encode(ref Span<byte> buffer, SignalPayload payload, ref int position)
    {
        buffer[position++] = payload.Strength;
    }
}
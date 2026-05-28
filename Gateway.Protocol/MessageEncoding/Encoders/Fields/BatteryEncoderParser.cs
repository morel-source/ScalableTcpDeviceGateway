using Gateway.Protocol.MessageEncoding.Interfaces;
using Gateway.Protocol.Payloads.Fields;

namespace Gateway.Protocol.MessageEncoding.Encoders.Fields;

public sealed class BatteryEncoderParser : IFieldEncoder<BatteryPayload>
{
    public void Encode(ref Span<byte> buffer, BatteryPayload payload, ref int position)
    {
        buffer[position++] = payload.Percentage;
    }
}
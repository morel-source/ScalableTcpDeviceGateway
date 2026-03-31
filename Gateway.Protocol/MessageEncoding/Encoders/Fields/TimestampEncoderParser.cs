using Gateway.Protocol.MessageEncoding.Interfaces;
using Gateway.Protocol.Payloads;

namespace Gateway.Protocol.MessageEncoding.Encoders.Fields;

public sealed class TimestampEncoderParser : IFieldEncoder<TimestampPayload>
{
    public void Encode(ref Span<byte> buffer, TimestampPayload payload, ref int position)
    {
        var dt = payload.Timestamp;

        buffer[position++] = (byte)(dt.Year - 2000);
        buffer[position++] = (byte)dt.Month;
        buffer[position++] = (byte)dt.Day;
        buffer[position++] = (byte)dt.Hour;
        buffer[position++] = (byte)dt.Minute;
        buffer[position++] = (byte)dt.Second;
    }
}
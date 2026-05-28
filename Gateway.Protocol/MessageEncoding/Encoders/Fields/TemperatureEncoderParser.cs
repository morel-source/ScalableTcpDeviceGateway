using System.Buffers.Binary;
using Gateway.Protocol.MessageEncoding.Interfaces;
using Gateway.Protocol.Payloads.Fields;

namespace Gateway.Protocol.MessageEncoding.Encoders.Fields;

public sealed class TemperatureEncoderParser : IFieldEncoder<TemperaturePayload>
{
    public void Encode(ref Span<byte> buffer, TemperaturePayload payload, ref int position)
    {
        // store as short = celsius * 10 
        var encoded = (short)(payload.Celsius * 10);
        BinaryPrimitives.WriteInt16BigEndian(buffer.Slice(position, 2), encoded);
        position += 2;
    }
}
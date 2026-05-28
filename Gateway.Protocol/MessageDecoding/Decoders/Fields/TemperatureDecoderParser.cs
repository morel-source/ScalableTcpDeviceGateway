using System.Buffers;
using Gateway.Protocol.MessageDecoding.Interfaces;
using Gateway.Protocol.Payloads.Fields;

namespace Gateway.Protocol.MessageDecoding.Decoders.Fields;

public sealed class TemperatureDecoderParser : IFieldDecoder<TemperaturePayload>
{
    public Result<TemperaturePayload> Decode(ref SequenceReader<byte> reader)
    {
        // temperature is stored as short (2 bytes) = celsius * 10
        // so 254 → 25.4°C
        if (!reader.TryReadBigEndian(out short raw))
            return Result<TemperaturePayload>.Failure("Failed to decode temperature");

        return Result<TemperaturePayload>.Success(
            new TemperaturePayload(raw / 10.0));
    }
}
using System.Buffers;
using Gateway.Protocol.MessageDecoding.Interfaces;
using Gateway.Protocol.Payloads.Fields;

namespace Gateway.Protocol.MessageDecoding.Decoders.Fields;

public sealed class BatteryDecoderParser : IFieldDecoder<BatteryPayload>
{
    public Result<BatteryPayload> Decode(ref SequenceReader<byte> reader)
    {
        if (!reader.TryRead(out byte percentage))
            return Result<BatteryPayload>.Failure("Failed to decode battery");

        return Result<BatteryPayload>.Success(new BatteryPayload(percentage));
    }
}
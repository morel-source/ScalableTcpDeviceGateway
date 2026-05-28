using System.Buffers;
using Gateway.Protocol.Enums;
using Gateway.Protocol.MessageDecoding.Interfaces;
using Gateway.Protocol.Payloads.Fields;

namespace Gateway.Protocol.MessageDecoding.Decoders.Fields;

public sealed class AlertTypeDecoderParser : IFieldDecoder<AlertTypePayload>
{
    public Result<AlertTypePayload> Decode(ref SequenceReader<byte> reader)
    {
        if (!reader.TryRead(out byte alertByte))
            return Result<AlertTypePayload>.Failure("Failed to decode alert type");

        if (!Enum.IsDefined(typeof(AlertType), alertByte))
            return Result<AlertTypePayload>.Failure($"Unknown alert type value: {alertByte}");

        return Result<AlertTypePayload>.Success(new AlertTypePayload((AlertType)alertByte));
    }
}
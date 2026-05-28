using System.Buffers;
using Gateway.Protocol.MessageDecoding.Base;
using Gateway.Protocol.MessageDecoding.Decoders.Frame;
using Gateway.Protocol.Payloads.Messages;

namespace Gateway.Protocol.MessageDecoding.Decoders.Messages;

public sealed class AckMessageDecoderParser(
    MessageTypeDecoderParser messageTypeDecoderParser
) : DecoderBase<AckMessagePayload>
{
    protected override Result<AckMessagePayload> Decode(ref SequenceReader<byte> reader)
    {
        var messageType = messageTypeDecoderParser.Decode(ref reader);
        if (!messageType.Ok)
            return Result<AckMessagePayload>.Failure("Failed to decode messageType");

        return Result<AckMessagePayload>.Success(new AckMessagePayload(messageType.Payload));
    }
}
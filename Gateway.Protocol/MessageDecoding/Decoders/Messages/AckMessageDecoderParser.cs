using System.Buffers;
using Gateway.Protocol.MessageDecoding.Base;
using Gateway.Protocol.MessageDecoding.Decoders.Frame;
using Gateway.Protocol.Payloads;

namespace Gateway.Protocol.MessageDecoding.Decoders.Messages;

public sealed class AckMessageDecoderParser(
    MessageTypeDecoderParser messageTypeDecoderParser
) : DecoderBase<AckPayload>
{
    protected override Result<AckPayload> Decode(ref SequenceReader<byte> reader)
    {
        var messageType = messageTypeDecoderParser.Decode(ref reader);
        if (!messageType.Ok)
            return Result<AckPayload>.Failure("Failed to decode messageType");

        return Result<AckPayload>.Success(new AckPayload(messageType.Payload));
    }
}
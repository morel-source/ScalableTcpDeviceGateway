using System.Buffers;
using Gateway.Protocol.Enums;
using Gateway.Protocol.MessageDecoding.Decoders.Frame;
using Gateway.Protocol.MessageDecoding.Interfaces;

namespace Gateway.Protocol.MessageDecoding;

public sealed class PacketDecoderParserHelper(
    HeaderDecoderParser headerDecoderParser,
    LengthDecoderParser lengthDecoderParser,
    MessageTypeDecoderParser messageTypeDecoderParser,
    FooterDecoderParser footerDecoderParser
) : IPacketDecoderParserHelper
{
    public bool GetPayloadBytesFromPacket(
        ref ReadOnlySequence<byte> sequence,
        out ReadOnlySequence<byte> body,
        out MessageType messageType)
    {
        var reader = new SequenceReader<byte>(sequence);
        messageType = MessageType.Unknown;
        body = ReadOnlySequence<byte>.Empty;

        var headerResult = headerDecoderParser.Decode(ref reader);
        if (!headerResult.Ok)
        {
            if (sequence.Length > 0)
                sequence = sequence.Slice(sequence.GetPosition(1));
            return false;
        }

        var messageTypeResult = messageTypeDecoderParser.Decode(ref reader);
        if (!messageTypeResult.Ok)
        {
            sequence = sequence.Slice(sequence.GetPosition(1));
            return false;
        }

        messageType = messageTypeResult.Payload;

        var lengthResult = lengthDecoderParser.Decode(ref reader);
        if (!lengthResult.Ok)
            return false;

        int dataLength = lengthResult.Payload;

        if (reader.Remaining < dataLength + 1)
            return false;

        SequencePosition bodyStart = reader.Position;
        reader.Advance(dataLength);
        SequencePosition bodyEnd = reader.Position;
        body = sequence.Slice(bodyStart, bodyEnd);

        var footerResult = footerDecoderParser.Decode(ref reader);
        if (!footerResult.Ok)
        {
            if (sequence.Length > 0)
                sequence = sequence.Slice(sequence.GetPosition(1));
            return false;
        }

        sequence = sequence.Slice(reader.Position);
        return true;
    }
}
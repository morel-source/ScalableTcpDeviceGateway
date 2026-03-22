using System.Buffers;
using Gateway.Protocol.Enums;
using Gateway.Protocol.MessageDecoding.Base;
using Gateway.Protocol.Payloads;

namespace Gateway.Protocol.MessageDecoding.Decoders.Messages;

public sealed class AckMessageDecoderParser : DecoderBase<AckPayload>
{
    public override AckPayload Decode(ref SequenceReader<byte> reader)
    {
        // Work on a copy so it don't mutate the original reader until it success to decode
        var internalReader = reader;

        if (!internalReader.TryRead(out byte start) || start != (byte)MessageType.StartByte)
            throw new InvalidDataException("Invalid Start Byte");

        if (!internalReader.TryRead(out byte type) || type != (byte)MessageType.Ack)
            throw new InvalidDataException("Invalid Message Type");

        if (!internalReader.TryRead(out byte end) || end != (byte)MessageType.EndByte)
            throw new InvalidDataException("Invalid End Byte");

        // Success! Now update the original reader to the new position
        reader = internalReader;

        return new AckPayload();
    }
}
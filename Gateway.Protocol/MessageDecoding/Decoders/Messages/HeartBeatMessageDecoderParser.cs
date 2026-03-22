using System.Buffers;
using Gateway.Protocol.Enums;
using Gateway.Protocol.MessageDecoding.Base;
using Gateway.Protocol.MessageDecoding.Base.Interfaces;
using Gateway.Protocol.MessageDecoding.Decoders.Fields;
using Gateway.Protocol.Payloads;

namespace Gateway.Protocol.MessageDecoding.Decoders.Messages;

public sealed class HeartBeatMessageDecoderParser(
    BarcodeDecoderParser barcodeDecoderParser,
    TimestampDecoderParser timestampDecoderParser
) : DecoderBase<HeartbeatPayload>
{
    public override HeartbeatPayload Decode(ref SequenceReader<byte> reader)
    {
        // Work on a copy so we don't mutate the original reader until we are sure
        var internalReader = reader;

        if (!internalReader.TryRead(out byte start) || start != (byte)MessageType.StartByte)
            throw new InvalidDataException("Invalid Start Byte");

        if (!internalReader.TryRead(out byte type) || type != (byte)MessageType.Heartbeat)
            throw new InvalidDataException("Invalid Message Type");

        // Delegate to field parsers using the internal reader
        var barcode = barcodeDecoderParser.Decode(ref internalReader);
        var timestamp = timestampDecoderParser.Decode(ref internalReader);

        if (!internalReader.TryRead(out byte end) || end != (byte)MessageType.EndByte)
            throw new InvalidDataException("Invalid End Byte");

        // Success! Now update the original reader to the new position
        reader = internalReader;

        return new HeartbeatPayload(barcode, timestamp);
    }
}
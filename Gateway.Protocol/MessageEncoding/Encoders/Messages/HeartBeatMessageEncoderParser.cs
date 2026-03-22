using Gateway.Protocol.Enums;
using Gateway.Protocol.MessageEncoding.Base.Interfaces;
using Gateway.Protocol.MessageEncoding.Encoders.Fields;
using Gateway.Protocol.Payloads;

namespace Gateway.Protocol.MessageEncoding.Encoders.Messages;

public sealed class HeartBeatMessageEncoderParser(
    BarcodeEncoderParser barcodeEncoderParser,
    TimestampEncoderParser timestampEncoderParser
) : IMessageEncoder<HeartbeatPayload>
{
    public byte[] Encode(HeartbeatPayload payload)
    {
        var buffer = new byte[HeartbeatPayload.FixedSize];
        var position = 0;

        buffer[position++] = (byte)MessageType.StartByte;
        buffer[position++] = (byte)MessageType.Heartbeat;

        barcodeEncoderParser.Encode(buffer, payload.DeviceBarcode, ref position);
        timestampEncoderParser.Encode(buffer, payload.Timestamp, ref position);

        buffer[position] = (byte)MessageType.EndByte;

        return buffer;
    }
}
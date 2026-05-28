using Gateway.Protocol.MessageEncoding.Base;
using Gateway.Protocol.MessageEncoding.Encoders.Fields;
using Gateway.Protocol.Payloads.Messages;

namespace Gateway.Protocol.MessageEncoding.Encoders.Messages;

public sealed class HeartBeatMessageEncoderParser(
    BarcodeEncoderParser barcodeEncoderParser,
    TimestampEncoderParser timestampEncoderParser
) : EncoderBase<HeartbeatMessagePayload>
{
    protected override void Encode(ref Span<byte> buffer, HeartbeatMessagePayload payload, ref int position)
    {
        barcodeEncoderParser.Encode(ref buffer, payload.DeviceBarcode, ref position);
        timestampEncoderParser.Encode(ref buffer, payload.Timestamp, ref position);
    }
}
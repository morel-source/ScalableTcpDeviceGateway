using Gateway.Protocol.MessageEncoding.Base;
using Gateway.Protocol.MessageEncoding.Encoders.Fields;
using Gateway.Protocol.Payloads.Messages;

namespace Gateway.Protocol.MessageEncoding.Encoders.Messages;

public sealed class AlertMessageEncoderParser(
    BarcodeEncoderParser barcodeEncoderParser,
    AlertTypeEncoderParser alertTypeEncoderParser,
    TimestampEncoderParser timestampEncoderParser
) : EncoderBase<AlertMessagePayload>
{
    protected override void Encode(ref Span<byte> buffer, AlertMessagePayload payload, ref int position)
    {
        barcodeEncoderParser.Encode(ref buffer, payload.DeviceBarcode, ref position);
        alertTypeEncoderParser.Encode(ref buffer, payload.AlertType, ref position);
        timestampEncoderParser.Encode(ref buffer, payload.Timestamp, ref position);
    }
}
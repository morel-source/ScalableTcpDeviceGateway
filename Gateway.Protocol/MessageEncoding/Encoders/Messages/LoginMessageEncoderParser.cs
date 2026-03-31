using Gateway.Protocol.MessageEncoding.Base;
using Gateway.Protocol.MessageEncoding.Encoders.Fields;
using Gateway.Protocol.Payloads;

namespace Gateway.Protocol.MessageEncoding.Encoders.Messages;

public sealed class LoginMessageEncoderParser(
    BarcodeEncoderParser barcodeEncoderParser,
    TimestampEncoderParser timestampEncoderParser
) : EncoderBase<LoginPayload>
{
    protected override void Encode(ref Span<byte> buffer, LoginPayload payload, ref int position)
    {
        barcodeEncoderParser.Encode(ref buffer, payload.DeviceBarcode, ref position);
        timestampEncoderParser.Encode(ref buffer, payload.Timestamp, ref position);
    }
}
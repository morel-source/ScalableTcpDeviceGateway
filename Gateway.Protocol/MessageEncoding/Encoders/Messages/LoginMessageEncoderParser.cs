using Gateway.Protocol.Enums;
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
        buffer[position++] = (byte)MessageType.StartByte;
        buffer[position++] = (byte)MessageType.Login;

        barcodeEncoderParser.Encode(buffer, payload.DeviceBarcode, ref position);
        timestampEncoderParser.Encode(buffer, payload.Timestamp, ref position);

        buffer[position++] = (byte)MessageType.EndByte;
    }
}
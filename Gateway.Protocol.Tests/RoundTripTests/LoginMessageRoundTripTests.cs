using Gateway.Protocol.MessageDecoding.Decoders.Messages;
using Gateway.Protocol.MessageEncoding.Encoders.Messages;
using Gateway.Protocol.Payloads.Fields;
using Gateway.Protocol.Payloads.Messages;
using Gateway.Protocol.Tests.RoundTripTests.Base;

namespace Gateway.Protocol.Tests.RoundTripTests;

public class LoginMessageRoundTripTests :
    RoundTripTestBase<LoginMessageEncoderParser, LoginMessageDecoderParser, LoginMessagePayload>
{
    protected override LoginMessagePayload SamplePayload => new(
        new BarcodePayload("000001"),
        new TimestampPayload(new DateTime(year: 2026, month: 05, day: 27, hour: 11, minute: 21, second: 43))
    );

    protected override byte[] Input =>
    [
        0x02, 0x01, 0x0C, 0x30, 0x30, 0x30, 0x30, 0x30, 0x31, 0x1A, 0x05, 0x1B, 0x0B, 0x15, 0x2B, 0x03
    ];
}
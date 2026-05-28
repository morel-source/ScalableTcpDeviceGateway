using Gateway.Protocol.Enums;
using Gateway.Protocol.MessageDecoding.Decoders.Messages;
using Gateway.Protocol.MessageEncoding.Encoders.Messages;
using Gateway.Protocol.Payloads.Messages;
using Gateway.Protocol.Tests.RoundTripTests.Base;

namespace Gateway.Protocol.Tests.RoundTripTests;

public class AlertMessageRoundTripTests : RoundTripTestBase<AlertMessageEncoderParser, AlertMessageDecoderParser,
    AlertMessagePayload>
{
    protected override AlertMessagePayload SamplePayload =>
        new(
            deviceBarcode: "000001",
            alertType: AlertType.HighTemperature,
            timestamp: new DateTime(year: 2026, month: 05, day: 28, hour: 10, minute: 45, second: 59)
        );

    protected override byte[] Input =>
    [
        0x02, 0x05, 0x0D, 0x30, 0x30, 0x30, 0x30, 0x30, 0x31, 0x01, 0x1A, 0x05, 0x1C, 0x0A, 0x2D, 0x3B, 0x03
    ];
}
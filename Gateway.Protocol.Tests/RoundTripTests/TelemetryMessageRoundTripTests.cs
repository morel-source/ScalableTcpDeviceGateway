using Gateway.Protocol.MessageDecoding.Decoders.Messages;
using Gateway.Protocol.MessageEncoding.Encoders.Messages;
using Gateway.Protocol.Payloads.Messages;
using Gateway.Protocol.Tests.RoundTripTests.Base;

namespace Gateway.Protocol.Tests.RoundTripTests;

public class TelemetryMessageRoundTripTests
    : RoundTripTestBase<TelemetryMessageEncoderParser, TelemetryMessageDecoderParser, TelemetryMessagePayload>
{
    protected override TelemetryMessagePayload SamplePayload => new(
        deviceBarcode: "000001",
        temperature: 74.5,
        battery: 96,
        signal: 43,
        timestamp: new DateTime(year: 2026, month: 05, day: 27, hour: 11, minute: 23, second: 44));

    protected override byte[] Input =>
    [
        0x02, 0x04, 0x10, 0x30, 0x30, 0x30, 0x30, 0x30, 0x31, 0x02, 0xE9, 0x60, 0x2B, 0x1A, 0x05, 0x1B, 0x0B,
        0x17, 0x2C, 0x03
    ];
}
using Gateway.Protocol.MessageDecoding.Decoders.Messages;
using Gateway.Protocol.MessageEncoding.Encoders.Messages;
using Gateway.Protocol.Payloads.Fields;
using Gateway.Protocol.Payloads.Messages;
using Gateway.Protocol.Tests.RoundTripTests.Base;

namespace Gateway.Protocol.Tests.RoundTripTests;

public class HeartBeatMessageRoundTripTests :
    RoundTripTestBase<HeartBeatMessageEncoderParser, HeartBeatMessageDecoderParser, HeartbeatMessagePayload>
{
    protected override HeartbeatMessagePayload SamplePayload => new(
        new BarcodePayload("000001"),
        new TimestampPayload(new DateTime(year: 2026, month: 05, day: 28, hour: 09, minute: 36, second: 24))
    );

    protected override byte[] Input =>
    [
        0x02, 0x02, 0x0C, 0x30, 0x30, 0x30, 0x30, 0x30, 0x31, 0x1A, 0x05, 0x1C, 0x09, 0x24, 0x18, 0x03
    ];
}
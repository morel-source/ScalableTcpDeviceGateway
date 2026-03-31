using Gateway.Protocol.MessageDecoding.Decoders.Messages;
using Gateway.Protocol.MessageEncoding.Encoders.Messages;
using Gateway.Protocol.Payloads;
using Gateway.Protocol.Tests.RoundTripTests.Base;

namespace Gateway.Protocol.Tests.RoundTripTests;

public class HeartBeatMessageRoundTripTests :
    RoundTripTestBase<HeartBeatMessageEncoderParser, HeartBeatMessageDecoderParser, HeartbeatPayload>
{
    protected override HeartbeatPayload SamplePayload => new(
        new BarcodePayload("123456"),
        new TimestampPayload(new DateTime(year: 2026, month: 03, day: 18, hour: 14, minute: 30, second: 05))
    );
}
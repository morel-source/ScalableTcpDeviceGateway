using Gateway.Protocol.MessageDecoding.Decoders.Messages;
using Gateway.Protocol.MessageEncoding.Encoders.Messages;
using Gateway.Protocol.Payloads;
using Gateway.Protocol.Tests.RoundTripTests.Base;

namespace Gateway.Protocol.Tests.RoundTripTests;

public class AckMessageRoundTripTests :
    RoundTripTestBase<AckMessageEncoderParser, AckMessageDecoderParser, AckPayload>
{
    protected override AckPayload SamplePayload => new();
}
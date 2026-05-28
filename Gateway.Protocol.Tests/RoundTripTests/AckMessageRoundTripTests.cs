using Gateway.Protocol.Enums;
using Gateway.Protocol.MessageDecoding.Decoders.Messages;
using Gateway.Protocol.MessageEncoding.Encoders.Messages;
using Gateway.Protocol.Payloads.Messages;
using Gateway.Protocol.Tests.RoundTripTests.Base;

namespace Gateway.Protocol.Tests.RoundTripTests;

public class AckMessageRoundTripTests :
    RoundTripTestBase<AckMessageEncoderParser, AckMessageDecoderParser, AckMessagePayload>
{
    protected override AckMessagePayload SamplePayload => new(MessageType.Login);
    protected override byte[] Input => [0x02, 0x03, 0x01, 0x01, 0x03];
}
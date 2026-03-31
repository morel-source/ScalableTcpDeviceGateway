using Gateway.Protocol.Enums;
using Gateway.Protocol.MessageDecoding.Decoders.Messages;
using Gateway.Protocol.Payloads;
using Gateway.Protocol.Tests.Common.Interfaces;
using Gateway.Protocol.Tests.DecodingTests.SuccessTests.Base;

namespace Gateway.Protocol.Tests.DecodingTests.SuccessTests.MessagesTests;

public class AckMessageDecoderParserTests :
    MessageDecoderTestBase<AckMessageDecoderParserTests, AckMessageDecoderParser, AckPayload>,
    ITestData<MessageDecoderTestBase<AckMessageDecoderParserTests, AckMessageDecoderParser, AckPayload>.TestCase>
{
    public static IEnumerable<TestCase> TheoryData =>
    [
        new(
            TestName: "Ack Test for Login Message",
            Input: [0x01, 0x22, 0x01, 0x11, 0x35],
            ExpectedResult: new AckPayload(MessageType.Login)),

        new(
            TestName: "Ack Test for Heartbeat Message",
            Input: [0x01, 0x22, 0x01, 0x12, 0x35],
            ExpectedResult: new AckPayload(MessageType.Heartbeat))
    ];
}
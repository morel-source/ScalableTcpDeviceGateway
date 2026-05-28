using Gateway.Protocol.Enums;
using Gateway.Protocol.MessageDecoding.Decoders.Messages;
using Gateway.Protocol.Payloads.Messages;
using Gateway.Protocol.Tests.Common.Interfaces;
using Gateway.Protocol.Tests.DecodingTests.SuccessTests.Base;

namespace Gateway.Protocol.Tests.DecodingTests.SuccessTests.MessagesTests;

public class AckMessageDecoderParserTests :
    MessageDecoderTestBase<AckMessageDecoderParserTests, AckMessageDecoderParser, AckMessagePayload>,
    ITestData<MessageDecoderTestBase<AckMessageDecoderParserTests, AckMessageDecoderParser, AckMessagePayload>.TestCase>
{
    public static IEnumerable<TestCase> TheoryData =>
    [
        new(
            TestName: "Ack Test for Login Message",
            Input: [0x02, 0x03, 0x01, 0x01, 0x03],
            ExpectedResult: new AckMessagePayload(MessageType.Login)),

        new(
            TestName: "Ack Test for Heartbeat Message",
            Input: [0x02, 0x03, 0x01, 0x02, 0x03],
            ExpectedResult: new AckMessagePayload(MessageType.Heartbeat)),

        new(
            TestName: "Ack Test for Telemetry Message",
            Input: [0x02, 0x03, 0x01, 0x04, 0x03],
            ExpectedResult: new AckMessagePayload(MessageType.Telemetry)),

        new(
            TestName: "Ack Test for Alert Message",
            Input: [0x02, 0x03, 0x01, 0x05, 0x03],
            ExpectedResult: new AckMessagePayload(MessageType.Alert))
    ];
}
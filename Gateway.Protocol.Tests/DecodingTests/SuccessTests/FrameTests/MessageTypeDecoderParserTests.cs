using Gateway.Protocol.Enums;
using Gateway.Protocol.MessageDecoding.Decoders.Frame;
using Gateway.Protocol.Tests.Common.Interfaces;
using Gateway.Protocol.Tests.DecodingTests.SuccessTests.Base;

namespace Gateway.Protocol.Tests.DecodingTests.SuccessTests.FrameTests;

public class MessageTypeDecoderParserTests :
    FrameDecoderTestBase<MessageTypeDecoderParserTests, MessageTypeDecoderParser, MessageType>,
    ITestData<FrameDecoderTestBase<MessageTypeDecoderParserTests, MessageTypeDecoderParser, MessageType>.TestCase>
{
    public static IEnumerable<TestCase> TheoryData =>
    [
        new(
            TestName: "Login MessageByte Test",
            Input: [0x11],
            ExpectedResult: MessageType.Login),

        new(
            TestName: "Heartbeat MessageByte Test",
            Input: [0x12],
            ExpectedResult: MessageType.Heartbeat),

        new(
            TestName: "Ack MessageByte Test",
            Input: [0x22],
            ExpectedResult: MessageType.Ack)
    ];
}
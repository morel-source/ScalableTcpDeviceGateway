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
            Input: [0x01],
            ExpectedResult: MessageType.Login),

        new(
            TestName: "Heartbeat MessageByte Test",
            Input: [0x02],
            ExpectedResult: MessageType.Heartbeat),

        new(
            TestName: "Ack MessageByte Test",
            Input: [0x03],
            ExpectedResult: MessageType.Ack),

        new(
            TestName: "Telemetry MessageByte Test",
            Input: [0x04],
            ExpectedResult: MessageType.Telemetry),

        new(
            TestName: "Alert MessageByte Test",
            Input: [0x05],
            ExpectedResult: MessageType.Alert)
    ];
}
using Gateway.Protocol.Enums;
using Gateway.Protocol.MessageEncoding.Encoders.Messages;
using Gateway.Protocol.MessageEncoding.Interfaces;
using Gateway.Protocol.Payloads;
using Gateway.Protocol.Tests.Common.Interfaces;
using Gateway.Protocol.Tests.EncodingTests.SuccessTests.Base;
using Microsoft.Extensions.DependencyInjection;

namespace Gateway.Protocol.Tests.EncodingTests.SuccessTests.MessagesTests;

public class AckMessageEncoderParserTests :
    MessageEncoderTestBase<AckMessageEncoderParserTests, AckMessageEncoderParser, AckPayload>,
    ITestData<MessageEncoderTestBase<AckMessageEncoderParserTests, AckMessageEncoderParser, AckPayload>.TestCase>
{
    protected override void AddDependencies(IServiceCollection services)
    {
        base.AddDependencies(services);
        services.AddKeyedSingleton<IMessageEncoder,AckMessageEncoderParser>(MessageType.Ack);
    }

    public static IEnumerable<TestCase> TheoryData =>
    [
        new(
            TestName: "Ack Test for Login Message",
            Input: new AckPayload(MessageType.Login),
            ExpectedBuffer: [0x01, 0x22, 0x01, 0x11, 0x35]),

        new(
            TestName: "Ack Test for HeartBeat Message",
            Input: new AckPayload(MessageType.Heartbeat),
            ExpectedBuffer: [0x01, 0x22, 0x01, 0x12, 0x35])
    ];
}
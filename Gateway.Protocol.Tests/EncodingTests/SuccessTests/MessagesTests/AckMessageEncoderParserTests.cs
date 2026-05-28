using Gateway.Protocol.Enums;
using Gateway.Protocol.MessageEncoding.Encoders.Messages;
using Gateway.Protocol.MessageEncoding.Interfaces;
using Gateway.Protocol.Payloads.Messages;
using Gateway.Protocol.Tests.Common.Interfaces;
using Gateway.Protocol.Tests.EncodingTests.SuccessTests.Base;
using Microsoft.Extensions.DependencyInjection;

namespace Gateway.Protocol.Tests.EncodingTests.SuccessTests.MessagesTests;

public class AckMessageEncoderParserTests :
    MessageEncoderTestBase<AckMessageEncoderParserTests, AckMessageEncoderParser, AckMessagePayload>,
    ITestData<MessageEncoderTestBase<AckMessageEncoderParserTests, AckMessageEncoderParser, AckMessagePayload>.TestCase>
{
    protected override void AddDependencies(IServiceCollection services)
    {
        base.AddDependencies(services);
        services.AddKeyedSingleton<IMessageEncoder, AckMessageEncoderParser>(MessageType.Ack);
    }

    public static IEnumerable<TestCase> TheoryData =>
    [
        new(
            TestName: "Ack Test for Login Message",
            ExpectedBuffer: [0x02, 0x03, 0x01, 0x01, 0x03],
            Input: new AckMessagePayload(MessageType.Login)),

        new(
            TestName: "Ack Test for Heartbeat Message",
            ExpectedBuffer: [0x02, 0x03, 0x01, 0x02, 0x03],
            Input: new AckMessagePayload(MessageType.Heartbeat)),

        new(
            TestName: "Ack Test for Telemetry Message",
            ExpectedBuffer: [0x02, 0x03, 0x01, 0x04, 0x03],
            Input: new AckMessagePayload(MessageType.Telemetry)),

        new(
            TestName: "Ack Test for Alert Message",
            ExpectedBuffer: [0x02, 0x03, 0x01, 0x05, 0x03],
            Input: new AckMessagePayload(MessageType.Alert))
    ];
}
using Gateway.Protocol.MessageEncoding.Encoders.Messages;
using Gateway.Protocol.Payloads;
using Gateway.Protocol.Tests.Common.Interfaces;
using Gateway.Protocol.Tests.EncodingTests.Base;
using Microsoft.Extensions.DependencyInjection;

namespace Gateway.Protocol.Tests.EncodingTests.MessagesTests;

public class AckMessageEncoderParserTests :
    MessageEncoderTestBase<AckMessageEncoderParserTests, AckMessageEncoderParser, AckPayload>,
    ITestData<MessageEncoderTestBase<AckMessageEncoderParserTests, AckMessageEncoderParser, AckPayload>.TestCase>
{
    protected override void AddDependencies(IServiceCollection services)
    {
        services.AddSingleton<AckMessageEncoderParser>();
    }

    public static IEnumerable<TestCase> TheoryData =>
    [
        new(
            TestName: "Ack Test",
            Input: new AckPayload(),
            ExpectedBuffer: [0x01, 0x22, 0x35])
    ];
}
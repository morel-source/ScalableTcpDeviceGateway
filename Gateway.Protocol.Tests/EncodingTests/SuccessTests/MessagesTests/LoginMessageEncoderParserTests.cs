using Gateway.Protocol.Enums;
using Gateway.Protocol.MessageEncoding.Encoders.Fields;
using Gateway.Protocol.MessageEncoding.Encoders.Messages;
using Gateway.Protocol.MessageEncoding.Interfaces;
using Gateway.Protocol.Payloads.Fields;
using Gateway.Protocol.Payloads.Messages;
using Gateway.Protocol.Tests.Common.Interfaces;
using Gateway.Protocol.Tests.EncodingTests.SuccessTests.Base;
using Microsoft.Extensions.DependencyInjection;

namespace Gateway.Protocol.Tests.EncodingTests.SuccessTests.MessagesTests;

public class LoginMessageEncoderParserTests :
    MessageEncoderTestBase<LoginMessageEncoderParserTests, LoginMessageEncoderParser, LoginMessagePayload>,
    ITestData<MessageEncoderTestBase<LoginMessageEncoderParserTests, LoginMessageEncoderParser, LoginMessagePayload>.
        TestCase>
{
    protected override void AddDependencies(IServiceCollection services)
    {
        base.AddDependencies(services);
        services.AddKeyedSingleton<IMessageEncoder, LoginMessageEncoderParser>(MessageType.Login);
        services.AddSingleton<BarcodeEncoderParser>();
        services.AddSingleton<TimestampEncoderParser>();
    }

    public static IEnumerable<TestCase> TheoryData =>
    [
        new(
            TestName: "Login Test",
            ExpectedBuffer:
            [
                0x02, 0x01, 0x0C, 0x30, 0x30, 0x30, 0x30, 0x30, 0x31, 0x1A, 0x05, 0x1B, 0x0B, 0x15, 0x2B, 0x03
            ],
            Input: new LoginMessagePayload(
                new BarcodePayload("000001"),
                new TimestampPayload(
                    new DateTime(year: 2026, month: 05, day: 27, hour: 11, minute: 21, second: 43
                    ))
            ))
    ];
}
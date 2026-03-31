using Gateway.Protocol.Enums;
using Gateway.Protocol.MessageEncoding.Encoders.Fields;
using Gateway.Protocol.MessageEncoding.Encoders.Messages;
using Gateway.Protocol.MessageEncoding.Interfaces;
using Gateway.Protocol.Payloads;
using Gateway.Protocol.Tests.Common.Interfaces;
using Gateway.Protocol.Tests.EncodingTests.SuccessTests.Base;
using Microsoft.Extensions.DependencyInjection;

namespace Gateway.Protocol.Tests.EncodingTests.SuccessTests.MessagesTests;

public class LoginMessageEncoderParserTests :
    MessageEncoderTestBase<LoginMessageEncoderParserTests, LoginMessageEncoderParser, LoginPayload>,
    ITestData<MessageEncoderTestBase<LoginMessageEncoderParserTests, LoginMessageEncoderParser, LoginPayload>.TestCase>
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
            Input: new LoginPayload(
                new BarcodePayload("000001"),
                new TimestampPayload(new DateTime(year: 2026, month: 03, day: 29, hour: 18, minute: 11, second: 27))),
            ExpectedBuffer: [0x01, 0x11, 0x0C, 0x30, 0x30, 0x30, 0x30, 0x30, 0x31, 0x1A, 0x03, 0x1D, 0x12, 0x0B, 0x1B, 0x35])
    ];
}
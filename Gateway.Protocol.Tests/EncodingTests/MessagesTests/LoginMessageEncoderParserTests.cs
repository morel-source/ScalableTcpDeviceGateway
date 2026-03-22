using Gateway.Protocol.MessageEncoding.Encoders.Fields;
using Gateway.Protocol.MessageEncoding.Encoders.Messages;
using Gateway.Protocol.Payloads;
using Gateway.Protocol.Tests.Common.Interfaces;
using Gateway.Protocol.Tests.EncodingTests.Base;
using Microsoft.Extensions.DependencyInjection;

namespace Gateway.Protocol.Tests.EncodingTests.MessagesTests;

public class LoginMessageEncoderParserTests :
    MessageEncoderTestBase<LoginMessageEncoderParserTests, LoginMessageEncoderParser, LoginPayload>,
    ITestData<MessageEncoderTestBase<LoginMessageEncoderParserTests, LoginMessageEncoderParser, LoginPayload>.TestCase>
{
    protected override void AddDependencies(IServiceCollection services)
    {
        services.AddSingleton<BarcodeEncoderParser>();
        services.AddSingleton<TimestampEncoderParser>();
    }

    public static IEnumerable<TestCase> TheoryData =>
    [
        new(
            TestName: "Login Test",
            Input: new LoginPayload(
                new BarcodePayload("000001"),
                new TimestampPayload(new DateTime(year: 2026, month: 03, day: 18, hour: 02, minute: 02, second: 25))),
            ExpectedBuffer: [0x01, 0x11, 0x30, 0x30, 0x30, 0x30, 0x30, 0x31, 0x1A, 0x03, 0x12, 0x02, 0x02, 0x19, 0x35])
    ];
}
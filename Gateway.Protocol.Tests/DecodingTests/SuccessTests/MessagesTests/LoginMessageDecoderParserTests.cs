using Gateway.Protocol.MessageDecoding.Decoders.Fields;
using Gateway.Protocol.MessageDecoding.Decoders.Messages;
using Gateway.Protocol.Payloads.Fields;
using Gateway.Protocol.Payloads.Messages;
using Gateway.Protocol.Tests.Common.Interfaces;
using Gateway.Protocol.Tests.DecodingTests.SuccessTests.Base;
using Microsoft.Extensions.DependencyInjection;

namespace Gateway.Protocol.Tests.DecodingTests.SuccessTests.MessagesTests;

public class LoginMessageDecoderParserTests :
    MessageDecoderTestBase<LoginMessageDecoderParserTests, LoginMessageDecoderParser, LoginMessagePayload>,
    ITestData<MessageDecoderTestBase<LoginMessageDecoderParserTests, LoginMessageDecoderParser, LoginMessagePayload>.
        TestCase>
{
    protected override void AddDependencies(IServiceCollection services)
    {
        base.AddDependencies(services);
        services.AddSingleton<BarcodeDecoderParser>();
        services.AddSingleton<TimestampDecoderParser>();
    }

    public static IEnumerable<TestCase> TheoryData =>
    [
        new(
            TestName: "Login Test",
            Input:
            [
                0x02, 0x01, 0x0C, 0x30, 0x30, 0x30, 0x30, 0x30, 0x31, 0x1A, 0x05, 0x1B, 0x0B, 0x15, 0x2B, 0x03
            ],
            ExpectedResult: new LoginMessagePayload(
                new BarcodePayload("000001"),
                new TimestampPayload(
                    new DateTime(year: 2026, month: 05, day: 27, hour: 11, minute: 21, second: 43))
            ))
    ];
}
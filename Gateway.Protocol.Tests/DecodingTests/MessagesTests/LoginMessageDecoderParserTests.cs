using Gateway.Protocol.MessageDecoding.Decoders.Fields;
using Gateway.Protocol.MessageDecoding.Decoders.Messages;
using Gateway.Protocol.Payloads;
using Gateway.Protocol.Tests.Common.Interfaces;
using Gateway.Protocol.Tests.DecodingTests.Base;
using Microsoft.Extensions.DependencyInjection;

namespace Gateway.Protocol.Tests.DecodingTests.MessagesTests;

public class LoginMessageDecoderParserTests :
    MessageDecoderTestBase<LoginMessageDecoderParserTests, LoginMessageDecoderParser, LoginPayload>,
    ITestData<MessageDecoderTestBase<LoginMessageDecoderParserTests, LoginMessageDecoderParser, LoginPayload>.TestCase>
{
    protected override void AddDependencies(IServiceCollection services)
    {
        services.AddSingleton<BarcodeDecoderParser>();
        services.AddSingleton<TimestampDecoderParser>();
    }

    public static IEnumerable<TestCase> TheoryData =>
    [
        new(
            TestName: "Login Test",
            Input: [0x01, 0x11, 0x30, 0x30, 0x30, 0x30, 0x30, 0x31, 0x1A, 0x03, 0x12, 0x02, 0x02, 0x19, 0x35],
            ExpectedResult: new LoginPayload(
                new BarcodePayload("000001"),
                new TimestampPayload(new DateTime(year: 2026, month: 03, day: 18, hour: 02, minute: 02, second: 25))))
    ];
}
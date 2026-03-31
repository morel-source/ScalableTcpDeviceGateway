using Gateway.Protocol.MessageEncoding.Encoders.Fields;
using Gateway.Protocol.Payloads;
using Gateway.Protocol.Tests.Common.Interfaces;
using Gateway.Protocol.Tests.EncodingTests.SuccessTests.Base;
using Microsoft.Extensions.DependencyInjection;

namespace Gateway.Protocol.Tests.EncodingTests.SuccessTests.FieldsTests;

public class TimestampEncoderParserTests :
    FieldEncoderTestBase<TimestampEncoderParserTests, TimestampEncoderParser, TimestampPayload>,
    ITestData<FieldEncoderTestBase<TimestampEncoderParserTests, TimestampEncoderParser, TimestampPayload>.TestCase>
{
    protected override void AddDependencies(IServiceCollection services)
    {
        services.AddSingleton<TimestampEncoderParser>();
    }

    public static IEnumerable<TestCase> TheoryData =>
    [
        new(
            TestName: "Timestamp Test",
            Input: new TimestampPayload(new DateTime(year: 2026, month: 03, day: 18, hour: 14, minute: 30, second: 05)),
            ExpectedBuffer: [0x1A, 0x03, 0x12, 0x0E, 0x1E, 0x05])
    ];
}
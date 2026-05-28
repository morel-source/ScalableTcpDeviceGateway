using Gateway.Protocol.MessageEncoding.Encoders.Fields;
using Gateway.Protocol.Payloads.Fields;
using Gateway.Protocol.Tests.Common.Interfaces;
using Gateway.Protocol.Tests.EncodingTests.SuccessTests.Base;

namespace Gateway.Protocol.Tests.EncodingTests.SuccessTests.FieldsTests;

public class TemperatureEncoderParserTests
    : FieldEncoderTestBase<TemperatureEncoderParserTests, TemperatureEncoderParser, TemperaturePayload>,
        ITestData<FieldEncoderTestBase<TemperatureEncoderParserTests, TemperatureEncoderParser, TemperaturePayload>.
            TestCase>
{
    public static IEnumerable<TestCase> TheoryData =>
    [
        new(
            TestName: "Temperature Test",
            ExpectedBuffer: [0x00, 0x00],
            Input: new TemperaturePayload(Celsius: 0)),
        new(
            TestName: "Temperature Test",
            ExpectedBuffer: [0x00, 0x4E],
            Input: new TemperaturePayload(Celsius: 7.8)),
        new(
            TestName: "Temperature Test",
            ExpectedBuffer: [0x00, 0x64],
            Input: new TemperaturePayload(Celsius: 10))
    ];
}
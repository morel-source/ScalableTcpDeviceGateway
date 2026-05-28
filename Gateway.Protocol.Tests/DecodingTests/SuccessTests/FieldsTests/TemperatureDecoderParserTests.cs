using Gateway.Protocol.MessageDecoding.Decoders.Fields;
using Gateway.Protocol.Payloads.Fields;
using Gateway.Protocol.Tests.Common.Interfaces;
using Gateway.Protocol.Tests.DecodingTests.SuccessTests.Base;

namespace Gateway.Protocol.Tests.DecodingTests.SuccessTests.FieldsTests;

public class TemperatureDecoderParserTests
    : FieldDecoderTestBase<TemperatureDecoderParserTests, TemperatureDecoderParser, TemperaturePayload>,
        ITestData<FieldDecoderTestBase<TemperatureDecoderParserTests, TemperatureDecoderParser, TemperaturePayload>.
            TestCase>
{
    public static IEnumerable<TestCase> TheoryData =>
    [
        new(
            TestName: "Temperature Test",
            Input: [0x00, 0x00],
            ExpectedResult: new TemperaturePayload(Celsius: 0)),
        new(
            TestName: "Temperature Test",
            Input: [0x00, 0x4E],
            ExpectedResult: new TemperaturePayload(Celsius: 7.8)),
        new(
            TestName: "Temperature Test",
            Input: [0x00, 0x64],
            ExpectedResult: new TemperaturePayload(Celsius: 10))
    ];
}
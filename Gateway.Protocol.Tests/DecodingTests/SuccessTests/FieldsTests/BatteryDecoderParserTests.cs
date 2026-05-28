using Gateway.Protocol.MessageDecoding.Decoders.Fields;
using Gateway.Protocol.Payloads.Fields;
using Gateway.Protocol.Tests.Common.Interfaces;
using Gateway.Protocol.Tests.DecodingTests.SuccessTests.Base;

namespace Gateway.Protocol.Tests.DecodingTests.SuccessTests.FieldsTests;

public class BatteryDecoderParserTests :
    FieldDecoderTestBase<BatteryDecoderParserTests, BatteryDecoderParser, BatteryPayload>,
    ITestData<FieldDecoderTestBase<BatteryDecoderParserTests, BatteryDecoderParser, BatteryPayload>.TestCase>
{
    public static IEnumerable<TestCase> TheoryData =>
    [
        new(
            TestName: "Battery Test",
            Input: [0x00],
            ExpectedResult: new BatteryPayload(Percentage: 0)),
        new(
            TestName: "Battery Test",
            Input: [0x4E],
            ExpectedResult: new BatteryPayload(Percentage: 78)),
        new(
            TestName: "Battery Test",
            Input: [0x64],
            ExpectedResult: new BatteryPayload(Percentage: 100))
    ];
}
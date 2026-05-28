using Gateway.Protocol.MessageEncoding.Encoders.Fields;
using Gateway.Protocol.Payloads.Fields;
using Gateway.Protocol.Tests.Common.Interfaces;
using Gateway.Protocol.Tests.EncodingTests.SuccessTests.Base;

namespace Gateway.Protocol.Tests.EncodingTests.SuccessTests.FieldsTests;

public class BatteryEncoderParserTests :
    FieldEncoderTestBase<BatteryEncoderParserTests, BatteryEncoderParser, BatteryPayload>,
    ITestData<FieldEncoderTestBase<BatteryEncoderParserTests, BatteryEncoderParser, BatteryPayload>.TestCase>
{
    public static IEnumerable<TestCase> TheoryData =>
    [
        new(
            TestName: "Battery Test",
            ExpectedBuffer: [0x00],
            Input: new BatteryPayload(Percentage: 0)),
        new(
            TestName: "Battery Test",
            ExpectedBuffer: [0x4E],
            Input: new BatteryPayload(Percentage: 78)),
        new(
            TestName: "Battery Test",
            ExpectedBuffer: [0x64],
            Input: new BatteryPayload(Percentage: 100))
    ];
}
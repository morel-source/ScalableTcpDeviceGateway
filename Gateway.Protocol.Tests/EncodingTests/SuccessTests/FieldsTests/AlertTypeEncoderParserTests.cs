using Gateway.Protocol.Enums;
using Gateway.Protocol.MessageEncoding.Encoders.Fields;
using Gateway.Protocol.Payloads.Fields;
using Gateway.Protocol.Tests.Common.Interfaces;
using Gateway.Protocol.Tests.EncodingTests.SuccessTests.Base;

namespace Gateway.Protocol.Tests.EncodingTests.SuccessTests.FieldsTests;

public class AlertTypeEncoderParserTests :
    FieldEncoderTestBase<AlertTypeEncoderParserTests, AlertTypeEncoderParser, AlertTypePayload>,
    ITestData<FieldEncoderTestBase<AlertTypeEncoderParserTests, AlertTypeEncoderParser, AlertTypePayload>.TestCase>
{
    public static IEnumerable<TestCase> TheoryData =>
    [
        new(
            TestName: "HighTemperature Alert Test",
            ExpectedBuffer: [0x01],
            Input: new AlertTypePayload(AlertType.HighTemperature)),

        new(
            TestName: "LowBattery Alert Test",
            ExpectedBuffer: [0x02],
            Input: new AlertTypePayload(AlertType.LowBattery)),

        new(
            TestName: "SignalLost Alert Test",
            ExpectedBuffer: [0x03],
            Input: new AlertTypePayload(AlertType.SignalLost))
    ];
}
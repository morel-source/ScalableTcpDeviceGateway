using Gateway.Protocol.Enums;
using Gateway.Protocol.MessageDecoding.Decoders.Fields;
using Gateway.Protocol.Payloads.Fields;
using Gateway.Protocol.Tests.Common.Interfaces;
using Gateway.Protocol.Tests.DecodingTests.SuccessTests.Base;

namespace Gateway.Protocol.Tests.DecodingTests.SuccessTests.FieldsTests;

public class AlertTypeDecoderParserTests :
    FieldDecoderTestBase<AlertTypeDecoderParserTests, AlertTypeDecoderParser, AlertTypePayload>,
    ITestData<FieldDecoderTestBase<AlertTypeDecoderParserTests, AlertTypeDecoderParser, AlertTypePayload>.TestCase>
{
    public static IEnumerable<TestCase> TheoryData =>
    [
        new(
            TestName: "HighTemperature Alert Test",
            Input: [0x01],
            ExpectedResult: new AlertTypePayload(AlertType.HighTemperature)),

        new(
            TestName: "LowBattery Alert Test",
            Input: [0x02],
            ExpectedResult: new AlertTypePayload(AlertType.LowBattery)),

        new(
            TestName: "SignalLost Alert Test",
            Input: [0x03],
            ExpectedResult: new AlertTypePayload(AlertType.SignalLost))
    ];
}
using Gateway.Protocol.MessageDecoding.Decoders.Fields;
using Gateway.Protocol.Payloads.Fields;
using Gateway.Protocol.Tests.Common.Interfaces;
using Gateway.Protocol.Tests.DecodingTests.SuccessTests.Base;

namespace Gateway.Protocol.Tests.DecodingTests.SuccessTests.FieldsTests;

public class SignalDecoderParserTests :
    FieldDecoderTestBase<SignalDecoderParserTests, SignalDecoderParser, SignalPayload>,
    ITestData<FieldDecoderTestBase<SignalDecoderParserTests, SignalDecoderParser, SignalPayload>.TestCase>
{
    public static IEnumerable<TestCase> TheoryData =>
    [
        new(
            TestName: "Strength Test",
            Input: [0x00],
            ExpectedResult: new SignalPayload(Strength: 0)),
        new(
            TestName: "Strength Test",
            Input: [0x4E],
            ExpectedResult: new SignalPayload(Strength: 78)),
        new(
            TestName: "Strength Test",
            Input: [0x64],
            ExpectedResult: new SignalPayload(Strength: 100))
    ];
}
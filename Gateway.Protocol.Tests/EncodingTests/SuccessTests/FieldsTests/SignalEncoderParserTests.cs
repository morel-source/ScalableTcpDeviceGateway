using Gateway.Protocol.MessageEncoding.Encoders.Fields;
using Gateway.Protocol.Payloads.Fields;
using Gateway.Protocol.Tests.Common.Interfaces;
using Gateway.Protocol.Tests.EncodingTests.SuccessTests.Base;

namespace Gateway.Protocol.Tests.EncodingTests.SuccessTests.FieldsTests;

public class SignalEncoderParserTests
    : FieldEncoderTestBase<SignalEncoderParserTests, SignalEncoderParser, SignalPayload>,
        ITestData<FieldEncoderTestBase<SignalEncoderParserTests, SignalEncoderParser, SignalPayload>.TestCase>

{
    public static IEnumerable<TestCase> TheoryData =>
    [
        new(
            TestName: "Strength Test",
            ExpectedBuffer: [0x00],
            Input: new SignalPayload(Strength: 0)),
        new(
            TestName: "Strength Test",
            ExpectedBuffer: [0x4E],
            Input: new SignalPayload(Strength: 78)),
        new(
            TestName: "Strength Test",
            ExpectedBuffer: [0x64],
            Input: new SignalPayload(Strength: 100))
    ];
}
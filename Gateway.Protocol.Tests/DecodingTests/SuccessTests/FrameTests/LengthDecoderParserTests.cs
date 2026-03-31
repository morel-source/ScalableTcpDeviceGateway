using Gateway.Protocol.MessageDecoding.Decoders.Frame;
using Gateway.Protocol.Tests.Common.Interfaces;
using Gateway.Protocol.Tests.DecodingTests.SuccessTests.Base;

namespace Gateway.Protocol.Tests.DecodingTests.SuccessTests.FrameTests;

public class LengthDecoderParserTests :
    FrameDecoderTestBase<LengthDecoderParserTests, LengthDecoderParser, int>,
    ITestData<FrameDecoderTestBase<LengthDecoderParserTests, LengthDecoderParser, int>.TestCase>
{
    public static IEnumerable<TestCase> TheoryData =>
    [
        new(
            TestName: "Length Test",
            Input: [0x0C],
            ExpectedResult: 12)
    ];
}
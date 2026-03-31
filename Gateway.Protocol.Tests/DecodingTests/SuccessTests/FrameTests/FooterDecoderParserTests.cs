using Gateway.Protocol.Enums;
using Gateway.Protocol.MessageDecoding.Decoders.Frame;
using Gateway.Protocol.Tests.Common.Interfaces;
using Gateway.Protocol.Tests.DecodingTests.SuccessTests.Base;

namespace Gateway.Protocol.Tests.DecodingTests.SuccessTests.FrameTests;

public class FooterDecoderParserTests :
    FrameDecoderTestBase<FooterDecoderParserTests, FooterDecoderParser, FrameByte>,
    ITestData<FrameDecoderTestBase<FooterDecoderParserTests, FooterDecoderParser, FrameByte>.TestCase>
{
    public static IEnumerable<TestCase> TheoryData =>
    [
        new(
            TestName: "Footer Test",
            Input: [0x35],
            ExpectedResult: FrameByte.EndByte)
    ];
}
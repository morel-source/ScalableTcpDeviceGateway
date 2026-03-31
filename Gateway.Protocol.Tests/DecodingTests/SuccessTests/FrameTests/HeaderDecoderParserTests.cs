using Gateway.Protocol.Enums;
using Gateway.Protocol.MessageDecoding.Decoders.Frame;
using Gateway.Protocol.Tests.Common.Interfaces;
using Gateway.Protocol.Tests.DecodingTests.SuccessTests.Base;

namespace Gateway.Protocol.Tests.DecodingTests.SuccessTests.FrameTests;

public class HeaderDecoderParserTests :
    FrameDecoderTestBase<HeaderDecoderParserTests, HeaderDecoderParser, FrameByte>,
    ITestData<FrameDecoderTestBase<HeaderDecoderParserTests, HeaderDecoderParser, FrameByte>.TestCase>
{
    public static IEnumerable<TestCase> TheoryData =>
    [
        new(
            TestName: "Header Test",
            Input: [0x01],
            ExpectedResult: FrameByte.StartByte)
    ];
}
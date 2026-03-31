using Gateway.Protocol.MessageDecoding.Decoders.Fields;
using Gateway.Protocol.Payloads;
using Gateway.Protocol.Tests.Common.Interfaces;
using Gateway.Protocol.Tests.DecodingTests.SuccessTests.Base;

namespace Gateway.Protocol.Tests.DecodingTests.SuccessTests.FieldsTests;

public class TimestampDecoderParserTests :
    FieldDecoderTestBase<TimestampDecoderParserTests, TimestampDecoderParser, TimestampPayload>,
    ITestData<FieldDecoderTestBase<TimestampDecoderParserTests, TimestampDecoderParser, TimestampPayload>.TestCase>
{
    public static IEnumerable<TestCase> TheoryData =>
    [
        new(
            TestName: "Timestamp Test",
            Input: [0x1A, 0x03, 0x12, 0x0E, 0x1E, 0x05],
            ExpectedResult: new TimestampPayload(new DateTime(year: 2026, month: 03, day: 18, hour: 14, minute: 30, second: 05)))
    ];
}
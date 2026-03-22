using Gateway.Protocol.MessageDecoding.Decoders.Fields;
using Gateway.Protocol.Payloads;
using Gateway.Protocol.Tests.Common.Interfaces;
using Gateway.Protocol.Tests.DecodingTests.Base;

namespace Gateway.Protocol.Tests.DecodingTests.FieldsTests;

public class BarcodeDecoderParserTests :
    FieldDecoderTestBase<BarcodeDecoderParserTests, BarcodeDecoderParser, BarcodePayload>,
    ITestData<FieldDecoderTestBase<BarcodeDecoderParserTests, BarcodeDecoderParser, BarcodePayload>.TestCase>
{
    public static IEnumerable<TestCase> TheoryData =>
    [
        new(
            TestName: "Barcode Test",
            Input: [0x30, 0x31, 0x32, 0x33, 0x34, 0x35],
            ExpectedResult: new BarcodePayload("012345"))
    ];
}
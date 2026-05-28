using Gateway.Protocol.MessageEncoding.Encoders.Fields;
using Gateway.Protocol.Payloads.Fields;
using Gateway.Protocol.Tests.Common.Interfaces;
using Gateway.Protocol.Tests.EncodingTests.SuccessTests.Base;

namespace Gateway.Protocol.Tests.EncodingTests.SuccessTests.FieldsTests;

public class BarcodeEncoderParserTests :
    FieldEncoderTestBase<BarcodeEncoderParserTests, BarcodeEncoderParser, BarcodePayload>,
    ITestData<FieldEncoderTestBase<BarcodeEncoderParserTests, BarcodeEncoderParser, BarcodePayload>.TestCase>
{
    public static IEnumerable<TestCase> TheoryData =>
    [
        new(
            TestName: "Barcode Test",
            Input: new BarcodePayload("012345"),
            ExpectedBuffer: [0x30, 0x31, 0x32, 0x33, 0x34, 0x35])
    ];
}
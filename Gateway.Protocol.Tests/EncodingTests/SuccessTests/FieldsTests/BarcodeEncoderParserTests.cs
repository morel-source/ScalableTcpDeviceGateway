using Gateway.Protocol.MessageEncoding.Encoders.Fields;
using Gateway.Protocol.Payloads;
using Gateway.Protocol.Tests.Common.Interfaces;
using Gateway.Protocol.Tests.EncodingTests.SuccessTests.Base;
using Microsoft.Extensions.DependencyInjection;

namespace Gateway.Protocol.Tests.EncodingTests.SuccessTests.FieldsTests;

public class BarcodeEncoderParserTests :
    FieldEncoderTestBase<BarcodeEncoderParserTests, BarcodeEncoderParser, BarcodePayload>,
    ITestData<FieldEncoderTestBase<BarcodeEncoderParserTests, BarcodeEncoderParser, BarcodePayload>.TestCase>
{
    protected override void AddDependencies(IServiceCollection services)
    {
        services.AddSingleton<BarcodeEncoderParser>();
    }

    public static IEnumerable<TestCase> TheoryData =>
    [
        new(
            TestName: "Barcode Test",
            Input: new BarcodePayload("012345"),
            ExpectedBuffer: [0x30, 0x31, 0x32, 0x33, 0x34, 0x35])
    ];
}
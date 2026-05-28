using Gateway.Protocol.Enums;
using Gateway.Protocol.MessageDecoding.Decoders.Fields;
using Gateway.Protocol.MessageDecoding.Decoders.Messages;
using Gateway.Protocol.Payloads.Messages;
using Gateway.Protocol.Tests.Common.Interfaces;
using Gateway.Protocol.Tests.DecodingTests.SuccessTests.Base;
using Microsoft.Extensions.DependencyInjection;

namespace Gateway.Protocol.Tests.DecodingTests.SuccessTests.MessagesTests;

public class AlertMessageDecoderParserTests
    : MessageDecoderTestBase<AlertMessageDecoderParserTests, AlertMessageDecoderParser, AlertMessagePayload>,
        ITestData<MessageDecoderTestBase<AlertMessageDecoderParserTests, AlertMessageDecoderParser, AlertMessagePayload>
            .TestCase>
{
    protected override void AddDependencies(IServiceCollection services)
    {
        base.AddDependencies(services);
        services.AddSingleton<BarcodeDecoderParser>();
        services.AddSingleton<AlertTypeDecoderParser>();
        services.AddSingleton<TimestampDecoderParser>();
    }

    public static IEnumerable<TestCase> TheoryData =>
    [
        new(
            TestName: "HighTemperature Alert Test Message",
            Input:
            [
                0x02, 0x05, 0x0D, 0x30, 0x30, 0x30, 0x30, 0x30, 0x31, 0x01, 0x1A, 0x05, 0x1C, 0x0A, 0x2D, 0x3B, 0x03
            ],
            ExpectedResult: new AlertMessagePayload(
                deviceBarcode: "000001",
                alertType: AlertType.HighTemperature,
                timestamp: new DateTime(year: 2026, month: 05, day: 28, hour: 10, minute: 45, second: 59)
            )
        ),
        new(
            TestName: "LowBattery Alert Test Message",
            Input:
            [
                0x02, 0x05, 0x0D, 0x30, 0x30, 0x30, 0x30, 0x30, 0x31, 0x02, 0x1A, 0x05, 0x1C, 0x0A, 0x32, 0x08, 0x03
            ],
            ExpectedResult: new AlertMessagePayload(
                deviceBarcode: "000001",
                alertType: AlertType.LowBattery,
                timestamp: new DateTime(year: 2026, month: 05, day: 28, hour: 10, minute: 50, second: 08)
            )
        ),
        new(
            TestName: "SignalLost Alert Test Message",
            Input:
            [
                0x02, 0x05, 0x0D, 0x30, 0x30, 0x30, 0x30, 0x30, 0x31, 0x03, 0x1A, 0x05, 0x1C, 0x0A, 0x2D, 0x1D, 0x03
            ],
            ExpectedResult: new AlertMessagePayload(
                deviceBarcode: "000001",
                alertType: AlertType.SignalLost,
                timestamp: new DateTime(year: 2026, month: 05, day: 28, hour: 10, minute: 45, second: 29)
            )
        )
    ];
}
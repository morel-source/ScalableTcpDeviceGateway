using Gateway.Protocol.Enums;
using Gateway.Protocol.MessageEncoding.Encoders.Fields;
using Gateway.Protocol.MessageEncoding.Encoders.Messages;
using Gateway.Protocol.MessageEncoding.Interfaces;
using Gateway.Protocol.Payloads.Messages;
using Gateway.Protocol.Tests.Common.Interfaces;
using Gateway.Protocol.Tests.EncodingTests.SuccessTests.Base;
using Microsoft.Extensions.DependencyInjection;

namespace Gateway.Protocol.Tests.EncodingTests.SuccessTests.MessagesTests;

public class TelemetryMessageEncoderParserTests : MessageEncoderTestBase<TelemetryMessageEncoderParserTests,
        TelemetryMessageEncoderParser, TelemetryMessagePayload>,
    ITestData<MessageEncoderTestBase<TelemetryMessageEncoderParserTests, TelemetryMessageEncoderParser,
        TelemetryMessagePayload>.TestCase>
{
    protected override void AddDependencies(IServiceCollection services)
    {
        base.AddDependencies(services);
        services.AddKeyedSingleton<IMessageEncoder, TelemetryMessageEncoderParser>(MessageType.Telemetry);
        services.AddSingleton<BarcodeEncoderParser>();
        services.AddSingleton<TemperatureEncoderParser>();
        services.AddSingleton<BatteryEncoderParser>();
        services.AddSingleton<SignalEncoderParser>();
        services.AddSingleton<TimestampEncoderParser>();
    }

    public static IEnumerable<TestCase> TheoryData =>
    [
        new(
            TestName: "Telemetry Test Message",
            ExpectedBuffer:
            [
                0x02, 0x04, 0x10, 0x30, 0x30, 0x30, 0x30, 0x30, 0x31, 0x02, 0xE9, 0x60, 0x2B, 0x1A, 0x05, 0x1B, 0x0B,
                0x17, 0x2C, 0x03
            ],
            Input: new TelemetryMessagePayload(
                deviceBarcode: "000001",
                temperature: 74.5,
                battery: 96,
                signal: 43,
                timestamp: new DateTime(year: 2026, month: 05, day: 27, hour: 11, minute: 23, second: 44)
            )
        )
    ];
}
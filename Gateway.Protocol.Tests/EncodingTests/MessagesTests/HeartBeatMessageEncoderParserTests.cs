using Gateway.Protocol.MessageEncoding.Encoders.Fields;
using Gateway.Protocol.MessageEncoding.Encoders.Messages;
using Gateway.Protocol.Payloads;
using Gateway.Protocol.Tests.Common.Interfaces;
using Gateway.Protocol.Tests.EncodingTests.Base;
using Microsoft.Extensions.DependencyInjection;

namespace Gateway.Protocol.Tests.EncodingTests.MessagesTests;

public class HeartBeatMessageEncoderParserTests :
    MessageEncoderTestBase<HeartBeatMessageEncoderParserTests, HeartBeatMessageEncoderParser, HeartbeatPayload>,
    ITestData<MessageEncoderTestBase<HeartBeatMessageEncoderParserTests, HeartBeatMessageEncoderParser,
        HeartbeatPayload>.TestCase>
{
    protected override void AddDependencies(IServiceCollection services)
    {
        services.AddSingleton<HeartBeatMessageEncoderParser>();
        services.AddSingleton<BarcodeEncoderParser>();
        services.AddSingleton<TimestampEncoderParser>();
    }

    public static IEnumerable<TestCase> TheoryData =>
    [
        new(
            TestName: "HeartBeat Test",
            Input: new HeartbeatPayload(
                new BarcodePayload("000001"),
                new TimestampPayload(new DateTime(year: 2026, month: 03, day: 18, hour: 14, minute: 30, second: 05))),
            ExpectedBuffer: [0x01, 0x12, 0x30, 0x30, 0x30, 0x030, 0x30, 0x31, 0x1A, 0x03, 0x12, 0x0E, 0x1E, 0x05, 0x035])
    ];
}
using Gateway.Protocol.Enums;
using Gateway.Protocol.MessageEncoding.Encoders.Fields;
using Gateway.Protocol.MessageEncoding.Encoders.Messages;
using Gateway.Protocol.MessageEncoding.Interfaces;
using Gateway.Protocol.Payloads;
using Gateway.Protocol.Tests.Common.Interfaces;
using Gateway.Protocol.Tests.EncodingTests.SuccessTests.Base;
using Microsoft.Extensions.DependencyInjection;

namespace Gateway.Protocol.Tests.EncodingTests.SuccessTests.MessagesTests;

public class HeartBeatMessageEncoderParserTests :
    MessageEncoderTestBase<HeartBeatMessageEncoderParserTests, HeartBeatMessageEncoderParser, HeartbeatPayload>,
    ITestData<MessageEncoderTestBase<HeartBeatMessageEncoderParserTests, HeartBeatMessageEncoderParser,
        HeartbeatPayload>.TestCase>
{
    protected override void AddDependencies(IServiceCollection services)
    {
        base.AddDependencies(services);
        services.AddKeyedSingleton<IMessageEncoder, HeartBeatMessageEncoderParser>(MessageType.Heartbeat);
        services.AddSingleton<BarcodeEncoderParser>();
        services.AddSingleton<TimestampEncoderParser>();
    }

    public static IEnumerable<TestCase> TheoryData =>
    [
        new(
            TestName: "HeartBeat Test",
            Input: new HeartbeatPayload(
                new BarcodePayload("000001"),
                new TimestampPayload(new DateTime(year: 2026, month: 03, day: 29, hour: 23, minute: 48, second: 53))),
            ExpectedBuffer: [0x01, 0x12, 0x0C, 0x30, 0x30, 0x30, 0x30, 0x30, 0x31, 0x1A, 0x03, 0x1D, 0x17, 0x30, 0x35, 0x35])
    ];
}
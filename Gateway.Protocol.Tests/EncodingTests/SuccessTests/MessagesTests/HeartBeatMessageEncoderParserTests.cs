using Gateway.Protocol.Enums;
using Gateway.Protocol.MessageEncoding.Encoders.Fields;
using Gateway.Protocol.MessageEncoding.Encoders.Messages;
using Gateway.Protocol.MessageEncoding.Interfaces;
using Gateway.Protocol.Payloads.Fields;
using Gateway.Protocol.Payloads.Messages;
using Gateway.Protocol.Tests.Common.Interfaces;
using Gateway.Protocol.Tests.EncodingTests.SuccessTests.Base;
using Microsoft.Extensions.DependencyInjection;

namespace Gateway.Protocol.Tests.EncodingTests.SuccessTests.MessagesTests;

public class HeartBeatMessageEncoderParserTests :
    MessageEncoderTestBase<HeartBeatMessageEncoderParserTests, HeartBeatMessageEncoderParser, HeartbeatMessagePayload>,
    ITestData<MessageEncoderTestBase<HeartBeatMessageEncoderParserTests, HeartBeatMessageEncoderParser,
        HeartbeatMessagePayload>.TestCase>
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
            ExpectedBuffer:
            [0x02, 0x02, 0x0C, 0x30, 0x30, 0x30, 0x30, 0x30, 0x31, 0x1A, 0x05, 0x1C, 0x09, 0x24, 0x18, 0x03],
            Input: new HeartbeatMessagePayload(
                new BarcodePayload("000001"),
                new TimestampPayload(new DateTime(year: 2026, month: 05, day: 28, hour: 09, minute: 36, second: 24))))
    ];
}
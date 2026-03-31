using Gateway.Protocol.MessageDecoding.Decoders.Fields;
using Gateway.Protocol.MessageDecoding.Decoders.Messages;
using Gateway.Protocol.Payloads;
using Gateway.Protocol.Tests.Common.Interfaces;
using Gateway.Protocol.Tests.DecodingTests.SuccessTests.Base;
using Microsoft.Extensions.DependencyInjection;

namespace Gateway.Protocol.Tests.DecodingTests.SuccessTests.MessagesTests;

public class HeartBeatMessageDecoderParserTests :
    MessageDecoderTestBase<HeartBeatMessageDecoderParserTests, HeartBeatMessageDecoderParser, HeartbeatPayload>,
    ITestData<MessageDecoderTestBase<HeartBeatMessageDecoderParserTests, HeartBeatMessageDecoderParser,
        HeartbeatPayload>.TestCase>
{
    protected override void AddDependencies(IServiceCollection services)
    {
        base.AddDependencies(services);
        services.AddSingleton<BarcodeDecoderParser>();
        services.AddSingleton<TimestampDecoderParser>();
    }

    public static IEnumerable<TestCase> TheoryData =>
    [
        new(
            TestName: "HeartBeat Test",
            Input: [0x01, 0x12, 0x0C, 0x30, 0x30, 0x30, 0x30, 0x30, 0x31, 0x1A, 0x03, 0x1D, 0x17, 0x30, 0x35, 0x35],
            ExpectedResult: new HeartbeatPayload(
                new BarcodePayload("000001"),
                new TimestampPayload(new DateTime(year: 2026, month: 03, day: 29, hour: 23, minute: 48, second: 53))))
    ];
}
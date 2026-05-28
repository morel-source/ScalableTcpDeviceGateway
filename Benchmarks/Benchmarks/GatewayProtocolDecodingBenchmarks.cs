using System.Buffers;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Gateway.Protocol.MessageDecoding;
using Gateway.Protocol.MessageDecoding.Decoders.Fields;
using Gateway.Protocol.MessageDecoding.Decoders.Frame;
using Gateway.Protocol.MessageDecoding.Decoders.Messages;
using Gateway.Protocol.MessageDecoding.Interfaces;
using Gateway.Protocol.Payloads.Messages;
using Microsoft.Extensions.DependencyInjection;

namespace Benchmarks.Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class GatewayProtocolDecodingBenchmarks
{
    private AckMessageDecoderParser _ackParser;
    private LoginMessageDecoderParser _loginParser;
    private HeartBeatMessageDecoderParser _heartbeatParser;
    private AlertMessageDecoderParser _alertParser;
    private TelemetryMessageDecoderParser _telemetryParser;

    private ReadOnlySequence<byte> _loginData;
    private ReadOnlySequence<byte> _heartbeatData;
    private ReadOnlySequence<byte> _ackData;
    private ReadOnlySequence<byte> _alertData;
    private ReadOnlySequence<byte> _telemetryData;

    private IPacketDecoderParserHelper _packetDecoderHelper;

    private ServiceProvider GetServiceCollection()
    {
        ServiceCollection services = new();
        services.AddSingleton<IPacketDecoderParserHelper, PacketDecoderParserHelper>();
        services.AddSingleton<BarcodeDecoderParser>();
        services.AddSingleton<TemperatureDecoderParser>();
        services.AddSingleton<BatteryDecoderParser>();
        services.AddSingleton<SignalDecoderParser>();
        services.AddSingleton<AlertTypeDecoderParser>();
        services.AddSingleton<TimestampDecoderParser>();

        services.AddSingleton<AckMessageDecoderParser>();
        services.AddSingleton<LoginMessageDecoderParser>();
        services.AddSingleton<HeartBeatMessageDecoderParser>();
        services.AddSingleton<AlertMessageDecoderParser>();
        services.AddSingleton<TelemetryMessageDecoderParser>();

        services.AddSingleton<HeaderDecoderParser>();
        services.AddSingleton<MessageTypeDecoderParser>();
        services.AddSingleton<LengthDecoderParser>();
        services.AddSingleton<FooterDecoderParser>();
        return services.BuildServiceProvider();
    }


    [GlobalSetup]
    public void Setup()
    {
        var serviceProvider = GetServiceCollection();
        _packetDecoderHelper = serviceProvider.GetRequiredService<IPacketDecoderParserHelper>();
        _ackParser = serviceProvider.GetRequiredService<AckMessageDecoderParser>();
        _heartbeatParser = serviceProvider.GetRequiredService<HeartBeatMessageDecoderParser>();
        _loginParser = serviceProvider.GetRequiredService<LoginMessageDecoderParser>();
        _alertParser = serviceProvider.GetRequiredService<AlertMessageDecoderParser>();
        _telemetryParser = serviceProvider.GetRequiredService<TelemetryMessageDecoderParser>();

        _loginData = new ReadOnlySequence<byte>(array:
        [
            0x02, 0x01, 0x0C, 0x30, 0x30, 0x30, 0x30, 0x30, 0x31, 0x1A, 0x05, 0x1B, 0x0B, 0x15, 0x2B, 0x03
        ]);
        _heartbeatData = new ReadOnlySequence<byte>(array:
        [
            0x02, 0x02, 0x0C, 0x30, 0x30, 0x30, 0x30, 0x30, 0x31, 0x1A, 0x05, 0x1C, 0x09, 0x24, 0x18, 0x03
        ]);
        _ackData = new ReadOnlySequence<byte>(array:
        [
            0x02, 0x03, 0x01, 0x01, 0x03
        ]);
        _alertData = new ReadOnlySequence<byte>(array:
        [
            0x02, 0x05, 0x0D, 0x30, 0x30, 0x30, 0x30, 0x30, 0x31, 0x01, 0x1A, 0x05, 0x1C, 0x0A, 0x2D, 0x3B, 0x03
        ]);
        _telemetryData = new ReadOnlySequence<byte>(array:
        [
            0x02, 0x04, 0x10, 0x30, 0x30, 0x30, 0x30, 0x30, 0x31, 0x02, 0xE9, 0x60, 0x2B, 0x1A, 0x05, 0x1B, 0x0B,
            0x17, 0x2C, 0x03
        ]);
    }

    [Benchmark]
    public Result<LoginMessagePayload> DecodeLogin()
    {
        _packetDecoderHelper.TryGetPayloadBytesFromPacket(ref _loginData, out var body, out var msgType);
        return _loginParser.Decode(body);
    }

    [Benchmark]
    public Result<HeartbeatMessagePayload> DecodeHeartbeat()
    {
        _packetDecoderHelper.TryGetPayloadBytesFromPacket(ref _heartbeatData, out var body, out var msgType);
        return _heartbeatParser.Decode(body);
    }

    [Benchmark]
    public Result<AckMessagePayload> DecodeAck()
    {
        _packetDecoderHelper.TryGetPayloadBytesFromPacket(ref _ackData, out var body, out var msgType);
        return _ackParser.Decode(body);
    }

    [Benchmark]
    public Result<AlertMessagePayload> DecodeAlert()
    {
        _packetDecoderHelper.TryGetPayloadBytesFromPacket(ref _alertData, out var body, out var msgType);
        return _alertParser.Decode(body);
    }

    [Benchmark]
    public Result<TelemetryMessagePayload> DecodeTelemetry()
    {
        _packetDecoderHelper.TryGetPayloadBytesFromPacket(ref _telemetryData, out var body, out var msgType);
        return _telemetryParser.Decode(body);
    }
}
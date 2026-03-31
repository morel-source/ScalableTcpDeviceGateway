using System.Buffers;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Gateway.Protocol.MessageDecoding;
using Gateway.Protocol.MessageDecoding.Decoders.Fields;
using Gateway.Protocol.MessageDecoding.Decoders.Frame;
using Gateway.Protocol.MessageDecoding.Decoders.Messages;
using Gateway.Protocol.MessageDecoding.Interfaces;
using Gateway.Protocol.Payloads;
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

    private ReadOnlySequence<byte> _loginData;
    private ReadOnlySequence<byte> _heartbeatData;
    private ReadOnlySequence<byte> _ackData;

    private IPacketDecoderParserHelper _packetDecoderHelper;

    private ServiceProvider GetServiceCollection()
    {
        ServiceCollection services = new();
        services.AddSingleton<IPacketDecoderParserHelper, PacketDecoderParserHelper>();
        services.AddSingleton<BarcodeDecoderParser>();
        services.AddSingleton<TimestampDecoderParser>();
        services.AddSingleton<MessageTypeDecoderParser>();
        services.AddSingleton<AckMessageDecoderParser>();
        services.AddSingleton<LoginMessageDecoderParser>();
        services.AddSingleton<HeartBeatMessageDecoderParser>();
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

        _loginData = new ReadOnlySequence<byte>(array:
        [
            0x01, 0x11, 0x0C, 0x30, 0x30, 0x30, 0x30, 0x30, 0x31, 0x1A, 0x03, 0x1D, 0x12, 0x0B, 0x1B, 0x35
        ]);
        _heartbeatData = new ReadOnlySequence<byte>(array:
        [
            0x01, 0x12, 0x0C, 0x30, 0x30, 0x30, 0x30, 0x30, 0x31, 0x1A, 0x03, 0x1D, 0x17, 0x30, 0x35, 0x35
        ]);
        _ackData = new ReadOnlySequence<byte>(array:
        [
            0x01, 0x22, 0x01, 0x11, 0x35
        ]);
    }

    [Benchmark]
    public Result<LoginPayload> DecodeLogin()
    {
        _packetDecoderHelper.GetPayloadBytesFromPacket(ref _loginData, out var body, out var msgType);
        return _loginParser.Decode(body);
    }

    [Benchmark]
    public Result<HeartbeatPayload> DecodeHeartbeat()
    {
        _packetDecoderHelper.GetPayloadBytesFromPacket(ref _heartbeatData, out var body, out var msgType);
        return _heartbeatParser.Decode(body);
    }

    [Benchmark]
    public Result<AckPayload> DecodeAck()
    {
        _packetDecoderHelper.GetPayloadBytesFromPacket(ref _ackData, out var body, out var msgType);
        return _ackParser.Decode(body);
    }
}
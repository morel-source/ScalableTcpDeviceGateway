using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Gateway.Protocol.MessageEncoding.Encoders.Fields;
using Gateway.Protocol.MessageEncoding.Encoders.Messages;
using Gateway.Protocol.Payloads;

namespace Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class GatewayProtocolEncodingBenchmarks
{
    private static readonly BarcodeEncoderParser BarcodeParser = new();
    private static readonly TimestampEncoderParser TimestampParser = new();

    private readonly AckMessageEncoderParser _ackParser = new();
    private readonly LoginMessageEncoderParser _loginParser = new(BarcodeParser, TimestampParser);
    private readonly HeartBeatMessageEncoderParser _heartbeatParser = new(BarcodeParser, TimestampParser);

    private LoginPayload _loginPayload;
    private HeartbeatPayload _heartbeatPayload;
    private AckPayload _ackPayload;

    [GlobalSetup]
    public void Setup()
    {
        var barcodePayload = new BarcodePayload("123456");
        var timestampPayload = new TimestampPayload(DateTime.Now);
        _loginPayload = new LoginPayload(barcodePayload, timestampPayload);
        _heartbeatPayload = new HeartbeatPayload(barcodePayload, timestampPayload);
        _ackPayload = new AckPayload();
    }

    [Benchmark]
    public int EncodeLogin()
    {
        Span<byte> buffer = stackalloc byte[LoginPayload.FixedSize];
        return _loginParser.Encode(buffer, _loginPayload);
    }

    [Benchmark]
    public int EncodeHeartbeat()
    {
        Span<byte> buffer = stackalloc byte[HeartbeatPayload.FixedSize];
        return _heartbeatParser.Encode(buffer, _heartbeatPayload);
    }

    [Benchmark]
    public int EncodeAck()
    {
        Span<byte> buffer = stackalloc byte[AckPayload.FixedSize];
        return _ackParser.Encode(buffer, _ackPayload);
    }
}
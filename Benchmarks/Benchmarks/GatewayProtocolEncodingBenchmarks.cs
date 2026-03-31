using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Gateway.Protocol.MessageEncoding.Encoders.Fields;
using Gateway.Protocol.MessageEncoding.Encoders.Messages;
using Gateway.Protocol.Payloads;

namespace Benchmarks.Benchmarks;

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
    public void EncodeLogin()
    {
        Span<byte> buffer = stackalloc byte[_loginPayload.FixedSize + 4];
        int position = 0;
        _loginParser.Encode(ref buffer, _loginPayload, ref position);
    }

    [Benchmark]
    public void EncodeHeartbeat()
    {
        Span<byte> buffer = stackalloc byte[_heartbeatPayload.FixedSize + 4];
        int position = 0;
        _heartbeatParser.Encode(ref buffer, _heartbeatPayload, ref position);
    }

    [Benchmark]
    public void EncodeAck()
    {
        Span<byte> buffer = stackalloc byte[_ackPayload.FixedSize + 4];
        int position = 0;
        _ackParser.Encode(ref buffer, _ackPayload, ref position);
    }
}
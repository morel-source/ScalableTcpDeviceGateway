using System.Buffers;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Gateway.Protocol.MessageDecoding.Decoders.Fields;
using Gateway.Protocol.MessageDecoding.Decoders.Messages;
using Gateway.Protocol.Payloads;

namespace Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class GatewayProtocolDecodingBenchmarks
{
    private static readonly BarcodeDecoderParser BarcodeParser = new();
    private static readonly TimestampDecoderParser TimestampParser = new();

    private readonly AckMessageDecoderParser _ackParser = new();
    private readonly LoginMessageDecoderParser _loginParser = new(BarcodeParser, TimestampParser);
    private readonly HeartBeatMessageDecoderParser _heartbeatParser = new(BarcodeParser, TimestampParser);

    private ReadOnlySequence<byte> _loginData;
    private ReadOnlySequence<byte> _heartbeatData;
    private ReadOnlySequence<byte> _ackData;

    [GlobalSetup]
    public void Setup()
    {
        _loginData = new ReadOnlySequence<byte>(array:
        [
            0x01, 0x11, 0x30, 0x30, 0x30, 0x30, 0x30, 0x31, 0x1A, 0x03, 0x12, 0x02, 0x02, 0x19, 0x35
        ]);
        _heartbeatData = new ReadOnlySequence<byte>(array:
        [
            0x01, 0x12, 0x30, 0x30, 0x30, 0x030, 0x30, 0x31, 0x1A, 0x03, 0x12, 0x0E, 0x1E, 0x05, 0x035
        ]);
        _ackData = new ReadOnlySequence<byte>(array:
        [
            0x01, 0x22, 0x35
        ]);
    }

    [Benchmark]
    public LoginPayload DecodeLogin()
    {
        var reader = new SequenceReader<byte>(_loginData);
        return _loginParser.Decode(ref reader);
    }

    [Benchmark]
    public HeartbeatPayload DecodeHeartbeat()
    {
        var reader = new SequenceReader<byte>(_heartbeatData);
        return _heartbeatParser.Decode(ref reader);
    }

    [Benchmark]
    public AckPayload DecodeAck()
    {
        var reader = new SequenceReader<byte>(_ackData);
        return _ackParser.Decode(ref reader);
    }
}
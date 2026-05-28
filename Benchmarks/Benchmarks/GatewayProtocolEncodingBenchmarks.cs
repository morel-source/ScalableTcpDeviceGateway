using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Gateway.Protocol.Enums;
using Gateway.Protocol.MessageEncoding.Encoders.Fields;
using Gateway.Protocol.MessageEncoding.Encoders.Messages;
using Gateway.Protocol.Payloads.Fields;
using Gateway.Protocol.Payloads.Messages;

namespace Benchmarks.Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class GatewayProtocolEncodingBenchmarks
{
    private static readonly BarcodeEncoderParser BarcodeParser = new();
    private static readonly TimestampEncoderParser TimestampParser = new();
    private static readonly AlertTypeEncoderParser AlertTypeParser = new();
    private static readonly TemperatureEncoderParser TemperatureParser = new();
    private static readonly BatteryEncoderParser BatteryParser = new();
    private static readonly SignalEncoderParser SignalParser = new();

    private readonly AckMessageEncoderParser _ackParser = new();
    private readonly LoginMessageEncoderParser _loginParser = new(BarcodeParser, TimestampParser);
    private readonly HeartBeatMessageEncoderParser _heartbeatParser = new(BarcodeParser, TimestampParser);
    private readonly AlertMessageEncoderParser _alertParser = new(BarcodeParser, AlertTypeParser, TimestampParser);

    private readonly TelemetryMessageEncoderParser _telemetryParser =
        new(BarcodeParser, TemperatureParser, BatteryParser, SignalParser, TimestampParser);


    private LoginMessagePayload _loginPayload;
    private HeartbeatMessagePayload _heartbeatPayload;
    private AckMessagePayload _ackPayload;
    private AlertMessagePayload _alertPayload;
    private TelemetryMessagePayload _telemetryPayload;

    [GlobalSetup]
    public void Setup()
    {
        var barcodePayload = new BarcodePayload("123456");
        var timestampPayload = new TimestampPayload(DateTime.Now);
        var alertType = new AlertTypePayload(AlertType.LowBattery);
        var temperaturePayload = new TemperaturePayload(90);
        var batteryPayload = new BatteryPayload();
        var signalPayload = new SignalPayload();

        _loginPayload = new LoginMessagePayload(barcodePayload, timestampPayload);
        _heartbeatPayload = new HeartbeatMessagePayload(barcodePayload, timestampPayload);
        _ackPayload = new AckMessagePayload();
        _alertPayload = new AlertMessagePayload(barcodePayload, alertType, timestampPayload);
        _telemetryPayload = new TelemetryMessagePayload(barcodePayload, temperaturePayload, batteryPayload,
            signalPayload, timestampPayload);
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

    [Benchmark]
    public void EncodeAlert()
    {
        Span<byte> buffer = stackalloc byte[_alertPayload.FixedSize + 4];
        int position = 0;
        _alertParser.Encode(ref buffer, _alertPayload, ref position);
    }


    [Benchmark]
    public void EncodeTelemetry()
    {
        Span<byte> buffer = stackalloc byte[_telemetryPayload.FixedSize + 4];
        int position = 0;
        _telemetryParser.Encode(ref buffer, _telemetryPayload, ref position);
    }
}
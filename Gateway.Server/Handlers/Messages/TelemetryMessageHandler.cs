using Gateway.Monitoring.Services;
using Gateway.Protocol.Payloads.Messages;
using Gateway.Server.Connections;
using Gateway.Server.Handlers.Base;
using Kafka.Contracts.Events;
using Kafka.Producer;
using Microsoft.Extensions.Logging;

namespace Gateway.Server.Handlers.Messages;

public sealed class TelemetryMessageHandler(
    ILogger<TelemetryMessageHandler> logger,
    IMetricsService metrics,
    TelemetryKafkaProducer kafkaProducer
) : MessageHandlerBase<TelemetryMessagePayload>
{
    protected override async Task ProcessMessage(DeviceConnectionContext context, TelemetryMessagePayload payload,
        CancellationToken cancellationToken = default)
    {
        using (metrics.MeasureTelemetryProcess())
        {
            metrics.IncrementTelemetryCount();
            metrics.RecordTemperature(context.DeviceBarcode, payload.Temperature.Celsius);

            logger.LogInformation("[{DeviceBarcode}] Telemetry temp={Temp}°C bat={Bat}% sig={Sig}",
                context.DeviceBarcode, payload.Temperature.Celsius,
                payload.Battery.Percentage, payload.Signal.Strength);

            await kafkaProducer.PublishTelemetryAsync(new TelemetryEvent
            {
                DeviceId = payload.DeviceBarcode.Value,
                Temperature = payload.Temperature.Celsius,
                BatteryLevel = payload.Battery.Percentage,
                SignalStrength = payload.Signal.Strength,
                ReceivedAt = DateTimeOffset.UtcNow,
                GatewayInstance = Environment.MachineName,
            }, cancellationToken);
        }
    }
}
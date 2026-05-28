using Gateway.Monitoring.Services;
using Gateway.Protocol.Payloads.Messages;
using Gateway.Server.Connections;
using Gateway.Server.Handlers.Base;
using Kafka.Contracts.Events;
using Kafka.Producer;
using Microsoft.Extensions.Logging;

namespace Gateway.Server.Handlers.Messages;

public class AlertMessageHandler(
    ILogger<AlertMessageHandler> logger,
    TelemetryKafkaProducer kafkaProducer,
    IMetricsService metrics
) : MessageHandlerBase<AlertMessagePayload>
{
    protected override async Task ProcessMessage(DeviceConnectionContext context, AlertMessagePayload payload,
        CancellationToken cancellationToken = default)
    {
        metrics.IncrementAlertCount();

        logger.LogWarning("[{DeviceBarcode}] Alert: {AlertType}", context.DeviceBarcode, payload.AlertType.AlertType);

        await kafkaProducer.PublishAlertAsync(new DeviceAlertEvent
        {
            DeviceId = payload.DeviceBarcode.Value,
            AlertType = payload.AlertType.AlertType,
            Message = $"Device {payload.DeviceBarcode.Value} reported {payload.AlertType.AlertType}",
            Timestamp = DateTimeOffset.UtcNow,
        }, cancellationToken);
    }
}
using Gateway.Monitoring.Services;
using Gateway.Protocol.Enums;
using Gateway.Protocol.Payloads.Messages;
using Gateway.Server.Connections;
using Gateway.Server.Handlers.Base;
using Kafka.Contracts.Events;
using Kafka.Producer;
using Microsoft.Extensions.Logging;

namespace Gateway.Server.Handlers.Messages;

public class LoginMessageMessageHandler(
    ILogger<LoginMessageMessageHandler> logger,
    IMetricsService metrics,
    TelemetryKafkaProducer kafkaProducer
) : MessageHandlerBase<LoginMessagePayload>
{
    protected override async Task ProcessMessage(DeviceConnectionContext context, LoginMessagePayload payload,
        CancellationToken cancellationToken = default)
    {
        using (metrics.MeasureLoginProcess())
        {
            context.DeviceBarcode = payload.DeviceBarcode.Value;

            logger.LogInformation("Device {DeviceBarcode} logged in.", context.DeviceBarcode);
            metrics.IncrementLoginConnections();

            await kafkaProducer.PublishDeviceStatusAsync(new DeviceStatusEvent
            {
                DeviceId = context.DeviceBarcode,
                Status = DeviceStatusType.Connected,
                Timestamp = DateTimeOffset.UtcNow,
            }, cancellationToken);
        }
    }
}
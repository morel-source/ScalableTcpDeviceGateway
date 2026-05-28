using Gateway.Monitoring.Services;
using Gateway.Protocol.Enums;
using Gateway.Protocol.Extensions;
using Gateway.Protocol.Payloads.Messages;
using Gateway.Server.Connections;
using Gateway.Server.Handlers.Base;
using Kafka.Contracts.Events;
using Kafka.Producer;
using Microsoft.Extensions.Logging;

namespace Gateway.Server.Handlers.Messages;

public sealed class HeartbeatMessageMessageHandler(
    ILogger<HeartbeatMessageMessageHandler> logger,
    IMetricsService metrics,
    TelemetryKafkaProducer kafkaProducer 
) : MessageHandlerBase<HeartbeatMessagePayload>
{
    protected override async Task ProcessMessage(DeviceConnectionContext context, HeartbeatMessagePayload payload,
        CancellationToken cancellationToken = default)
    {
        using (metrics.MeasureHeartBeatProcess())
        {
            context.DeviceBarcode = payload.DeviceBarcode.Value;

            logger.LogInformation("[{DeviceBarcode}] [{MessageType}]", context.DeviceBarcode,
                payload.MessageType.GetName());
           
            metrics.IncrementHeartBeatConnections();

            await kafkaProducer.PublishDeviceStatusAsync(new DeviceStatusEvent
            {
                DeviceId = context.DeviceBarcode,
                Status = DeviceStatusType.Heartbeat,
                Timestamp = DateTimeOffset.UtcNow,
            }, cancellationToken);
        }
    }
}
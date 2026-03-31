using Gateway.Monitoring.Services;
using Gateway.Protocol.Extensions;
using Gateway.Protocol.Payloads;
using Gateway.Server.Connections;
using Gateway.Server.Handlers.Base;
using Microsoft.Extensions.Logging;

namespace Gateway.Server.Handlers;

public class HeartbeatMessageMessageHandler(
    ILogger<HeartbeatMessageMessageHandler> logger,
    IMetricsService metrics
) : MessageHandlerBase<HeartbeatPayload>
{
    protected override Task ProcessMessage(DeviceConnectionContext context, HeartbeatPayload payload,
        CancellationToken cancellationToken = default)
    {
        using (metrics.MeasureHeartBeatProcess())
        {
            context.DeviceBarcode = payload.DeviceBarcode.Value;
            logger.LogInformation(message: "[{DeviceBarcode}] [{MessageType}]", context.DeviceBarcode, payload.MessageType.GetName());
            metrics.IncrementHeartBeatConnections();
            return Task.CompletedTask;
        }
    }
}
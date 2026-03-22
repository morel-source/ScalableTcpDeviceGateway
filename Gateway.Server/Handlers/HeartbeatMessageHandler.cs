using Gateway.Monitoring.Services;
using Gateway.Protocol.Payloads;
using Gateway.Server.Connections;
using Gateway.Server.Handlers.Base;
using Microsoft.Extensions.Logging;

namespace Gateway.Server.Handlers;

public class HeartbeatMessageHandler(
    ILogger<HeartbeatMessageHandler> logger,
    IMetricsService metrics
) : MessageHandlerBase<HeartbeatPayload>
{
    protected override Task ProcessMessage(DeviceConnectionContext context, HeartbeatPayload payload,
        CancellationToken cancellationToken = default)
    {
        using (metrics.MeasureHeartBeatProcess())
        {
            context.DeviceBarcode = payload.DeviceBarcode.Barcode;
            logger.LogInformation("[{barcode} [Heartbeat]", context.DeviceBarcode);
            metrics.IncrementHeartBeatConnections();
            return Task.CompletedTask;
        }
    }
}
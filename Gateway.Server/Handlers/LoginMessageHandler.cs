using Gateway.Monitoring.Services;
using Gateway.Protocol.Payloads;
using Gateway.Server.Connections;
using Gateway.Server.Handlers.Base;
using Microsoft.Extensions.Logging;

namespace Gateway.Server.Handlers;

public class LoginMessageHandler(
    ILogger<LoginMessageHandler> logger,
    IMetricsService metrics
) : MessageHandlerBase<LoginPayload>
{
    protected override Task ProcessMessage(DeviceConnectionContext context, LoginPayload payload,
        CancellationToken cancellationToken = default)
    {
        using (metrics.MeasureLoginProcess())
        {
            context.DeviceBarcode = payload.DeviceBarcode.Value;
            logger.LogInformation("[{barcode}] [Login]", context.DeviceBarcode);
            metrics.IncrementLoginConnections();
            return Task.CompletedTask;
        }
    }
}
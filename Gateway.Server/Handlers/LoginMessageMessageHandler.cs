using Gateway.Monitoring.Services;
using Gateway.Protocol.Payloads;
using Gateway.Server.Connections;
using Gateway.Server.Handlers.Base;
using Microsoft.Extensions.Logging;

namespace Gateway.Server.Handlers;

public class LoginMessageMessageHandler(
    ILogger<LoginMessageMessageHandler> logger,
    IMetricsService metrics
) : MessageHandlerBase<LoginPayload>
{
    protected override Task ProcessMessage(DeviceConnectionContext context, LoginPayload payload,
        CancellationToken cancellationToken = default)
    {
        using (metrics.MeasureLoginProcess())
        {
            context.DeviceBarcode = payload.DeviceBarcode.Value;
            logger.LogInformation(message: "Device {DeviceBarcode} logged in successfully.", context.DeviceBarcode);
            metrics.IncrementLoginConnections();
            return Task.CompletedTask;
        }
    }
}
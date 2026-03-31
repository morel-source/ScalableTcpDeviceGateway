using Device.Simulator.Configuration;
using Device.Simulator.Networking;
using Gateway.Protocol.Enums;
using Gateway.Protocol.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Device.Simulator.Messaging;

public class TcpMessageSender(
    ILogger<TcpMessageSender> logger,
    IOptions<SimulatorOptions> options
) : IMessageSender
{
    public async Task<bool> SendWithRetryAsync(
        int position,
        DeviceConnectionContext context,
        MessageType messageType,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(message: "[{DeviceBarcode}] [{MessageType}] Send", context.DeviceBarcode,
            messageType.GetName());

        context.Writer.Advance(position);
        await context.Writer.FlushAsync(cancellationToken).ConfigureAwait(false);

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(options.Value.AckTimeout);

        var ackMessage = await context.AckMessageChanel.WaitForAckAsync(timeout.Token).ConfigureAwait(false);
        if (ackMessage.Received && ackMessage.MessageType == messageType)
        {
            logger.LogInformation(message: "[{DeviceBarcode}] [{MessageType}] ACK Received", context.DeviceBarcode,
                messageType.GetName());
            return true;
        }

        return false;
    }
}
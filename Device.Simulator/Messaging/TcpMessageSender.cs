using Device.Simulator.Configuration;
using Device.Simulator.Networking;
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
        string messageName,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[{device}] [{type}] Send", context.DeviceBarcode, messageName);

        context.Writer.Advance(position);
        await context.Writer.FlushAsync(cancellationToken).ConfigureAwait(false);

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(options.Value.AckTimeout);

        if (await context.WaitForAckAsync(timeout.Token).ConfigureAwait(false))
        {
            logger.LogInformation("[{device}] [{type}] ACK Received", context.DeviceBarcode, messageName);
            return true;
        }

        return false;
    }
}
using System.IO.Pipelines;
using Device.Simulator.Configuration;
using Device.Simulator.Networking;
using Gateway.Protocol.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Device.Simulator.Messaging;

public class TcpMessageSender(
    ILogger<TcpMessageSender> logger,
    IOptions<SimulatorOptions> options) : IMessageSender
{
    public async Task<bool> SendLoginMessageAsync(byte[] message, DeviceConnectionContext context,
        CancellationToken cancellationToken = default)
    {
        return await SendWithRetryAsync(message, context, messageName: "LOGIN", cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<bool> SendHeartbeatMessageAsync(byte[] message, DeviceConnectionContext context,
        CancellationToken cancellationToken = default)
    {
        return await SendWithRetryAsync(message, context, messageName: "HEARTBEAT", cancellationToken)
            .ConfigureAwait(false);
    }

    private async Task<bool> SendWithRetryAsync(
        byte[] message,
        DeviceConnectionContext context,
        string messageName,
        CancellationToken cancellationToken = default)
    {
        int retryCount = 0;

        while (retryCount < 3)
        {
            logger.LogHex(message, $"[{context.DeviceBarcode}] [{messageName}] Tx:");

            await SendMessageAsync(context.Writer, message, cancellationToken).ConfigureAwait(false);

            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeout.CancelAfter(options.Value.AckTimeout);

            if (await context.WaitForAckAsync(timeout.Token).ConfigureAwait(false))
            {
                if (logger.IsEnabled(LogLevel.Information))
                    logger.LogInformation("[{device}] [{type}] ACK Received]", context.DeviceBarcode,
                        messageName);
                return true;
            }

            retryCount++;
            logger.LogWarning("[{device}] [{type}] retry {retry}", context.DeviceBarcode,messageName, retryCount);
        }

        return false;
    }


    private async Task SendMessageAsync(PipeWriter writer, byte[] message,
        CancellationToken cancellationToken = default)
    {
        await writer.WriteAsync(message, cancellationToken).ConfigureAwait(false);
        await writer.FlushAsync(cancellationToken).ConfigureAwait(false);
    }
}
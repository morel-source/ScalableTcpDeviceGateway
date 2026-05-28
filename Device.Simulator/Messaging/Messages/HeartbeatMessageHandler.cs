using Device.Simulator.Configuration;
using Device.Simulator.Networking;
using Gateway.Protocol.Enums;
using Gateway.Protocol.Extensions;
using Gateway.Protocol.MessageEncoding.Interfaces;
using Gateway.Protocol.Payloads.Fields;
using Gateway.Protocol.Payloads.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Device.Simulator.Messaging.Messages;

public sealed class HeartbeatMessageHandler(
    ILogger<HeartbeatMessageHandler> logger,
    IOptions<SimulatorOptions> options,
    IMessageSender messageSender,
    IPacketEncoderParserHelper packetEncoderParserHelper
)
{
    public async Task SendHeartbeatLoopAsync(DeviceConnectionContext context,
        CancellationToken cancellationToken = default)
    {
        var messageType = MessageType.Heartbeat;
        using var timer = new PeriodicTimer(options.Value.HeartbeatInterval);

        try
        {
            while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false))
            {
                if (options.Value.SimulateDisconnections && Random.Shared.NextDouble() < 0.01)
                {
                    logger.LogInformation(message: "[{DeviceBarcode}] simulating random drop", context.DeviceBarcode);
                    break;
                }

                bool success = false;
                int retryCount = 0;

                var payload = new HeartbeatMessagePayload(new BarcodePayload(context.DeviceBarcode),
                    new TimestampPayload(DateTime.Now));

                while (retryCount < 3)
                {
                    var buffer = context.Writer.GetSpan(payload.FixedSize + 4);
                    var position = packetEncoderParserHelper.EncodePayloadBytesIntoPacket(ref buffer, payload);

                    success = await messageSender
                        .SendWithRetryAsync(position, context, messageType: messageType, cancellationToken)
                        .ConfigureAwait(false);

                    if (success)
                    {
                        logger.LogInformation("[{DeviceBarcode}] Heartbeat sent successfully", context.DeviceBarcode);
                        break;
                    }

                    retryCount++;
                    logger.LogWarning(message: "[{DeviceBarcode}] [{MessageType}] retry {Retry}", context.DeviceBarcode,
                        messageType.GetName(), retryCount);
                }

                if (success)
                {
                    continue;
                }

                logger.LogWarning(message: "[{DeviceBarcode}] [TIMEOUT] [{MessageType}] ACK missing",
                    context.DeviceBarcode, messageType.GetName());

                break; // close connection
            }
        }
        catch (OperationCanceledException)
        {
        }
    }
}
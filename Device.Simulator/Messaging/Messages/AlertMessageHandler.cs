using Device.Simulator.Networking;
using Gateway.Protocol.Enums;
using Gateway.Protocol.Extensions;
using Gateway.Protocol.MessageEncoding.Interfaces;
using Gateway.Protocol.Payloads.Messages;
using Microsoft.Extensions.Logging;

namespace Device.Simulator.Messaging.Messages;

public sealed class AlertMessageHandler(
    ILogger<LoginMessageHandler> logger,
    IMessageSender messageSender,
    IPacketEncoderParserHelper packetEncoderParserHelper)
{
    public async Task SendAlertAsync(DeviceConnectionContext context, AlertType alertType,
        CancellationToken cancellationToken = default)
    {
        var messageType = MessageType.Alert;
        int retryCount = 0;

        var payload = new AlertMessagePayload(deviceBarcode: context.DeviceBarcode, alertType: alertType,
            timestamp: DateTime.Now);

        while (retryCount < 3)
        {
            var buffer = context.Writer.GetSpan(payload.FixedSize + 4);
            var position = packetEncoderParserHelper.EncodePayloadBytesIntoPacket(ref buffer, payload);

            var success = await messageSender
                .SendWithRetryAsync(position, context, messageType, cancellationToken)
                .ConfigureAwait(false);

            if (success)
            {
                logger.LogInformation("[{DeviceBarcode}] [{AlertType}] Alert sent successfully", context.DeviceBarcode,
                    alertType);
                return;
            }

            retryCount++;
            logger.LogWarning("[{DeviceBarcode}] [{MessageType}] retry {Retry}",
                context.DeviceBarcode, messageType.GetName(), retryCount);
        }

        logger.LogWarning("[{DeviceBarcode}] Alert {AlertType} failed after retries",
            context.DeviceBarcode, alertType);
    }
}
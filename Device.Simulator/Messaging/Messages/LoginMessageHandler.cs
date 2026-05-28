using Device.Simulator.Networking;
using Gateway.Protocol.Enums;
using Gateway.Protocol.Extensions;
using Gateway.Protocol.MessageEncoding.Interfaces;
using Gateway.Protocol.Payloads.Fields;
using Gateway.Protocol.Payloads.Messages;
using Microsoft.Extensions.Logging;

namespace Device.Simulator.Messaging.Messages;

public sealed class LoginMessageHandler(
    ILogger<LoginMessageHandler> logger,
    IMessageSender messageSender,
    IPacketEncoderParserHelper packetEncoderParserHelper)
{
    public async Task<bool> SendLoginAsync(DeviceConnectionContext context,
        CancellationToken cancellationToken = default)
    {
        var messageType = MessageType.Login;

        var message = new LoginMessagePayload(
            new BarcodePayload(context.DeviceBarcode),
            new TimestampPayload(DateTime.Now));

        try
        {
            bool success = false;
            int retryCount = 0;

            while (retryCount < 3)
            {
                var buffer = context.Writer.GetSpan(message.FixedSize + 4);

                var position = packetEncoderParserHelper.EncodePayloadBytesIntoPacket(ref buffer, message);

                success = await messageSender
                    .SendWithRetryAsync(position, context, messageType: messageType, cancellationToken)
                    .ConfigureAwait(false);

                if (success)
                {
                    logger.LogInformation("[{DeviceBarcode}] Login sent successfully", context.DeviceBarcode);
                    break;
                }

                retryCount++;
                logger.LogWarning(message: "[{DeviceBarcode}] [{MessageType}] retry {Retry}", context.DeviceBarcode,
                    messageType.GetName(), retryCount);
            }

            return success;
        }
        catch (Exception ex) when (ex is OperationCanceledException or TaskCanceledException)
        {
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "[{DeviceBarcode}] [{MessageType}] failed", context.DeviceBarcode,
                nameof(messageType));
            return false;
        }
    }
}
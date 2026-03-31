using System.Buffers;
using Device.Simulator.Configuration;
using Device.Simulator.Networking;
using Gateway.Protocol.Enums;
using Gateway.Protocol.Extensions;
using Gateway.Protocol.MessageDecoding.Interfaces;
using Gateway.Protocol.MessageEncoding.Interfaces;
using Gateway.Protocol.Payloads;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Device.Simulator.Messaging;

public class MessageHandler(
    ILogger<MessageHandler> logger,
    IOptions<SimulatorOptions> options,
    IMessageSender messageSender,
    IServiceProvider serviceProvider,
    IPacketDecoderParserHelper packetDecoderDecoderHelper,
    IPacketEncoderParserHelper packetEncoderParserHelper
) : IMessageHandler
{
    public async Task<bool> SendLoginAsync(DeviceConnectionContext context,
        CancellationToken cancellationToken = default)
    {
        var messageType = MessageType.Login;

        var message = new LoginPayload(
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
                    break;

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

                var payload = new HeartbeatPayload(new BarcodePayload(context.DeviceBarcode),
                    new TimestampPayload(DateTime.Now));

                while (retryCount < 3)
                {
                    var buffer = context.Writer.GetSpan(payload.FixedSize + 4);
                    var position = packetEncoderParserHelper.EncodePayloadBytesIntoPacket(ref buffer, payload);

                    success = await messageSender
                        .SendWithRetryAsync(position, context, messageType: messageType, cancellationToken)
                        .ConfigureAwait(false);

                    if (success)
                        break;

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

    public bool TryParseAckFrame(ref ReadOnlySequence<byte> buffer, out MessageType messageType)
    {
        messageType = MessageType.Unknown;
        try
        {
            var success =
                packetDecoderDecoderHelper.GetPayloadBytesFromPacket(sequence: ref buffer, out var body, out var msgType);
         
            if (success && msgType != MessageType.Ack)
            {
                logger.LogError("buffer is not recognize as Ack message");
                return false;
            }

            var decoder = serviceProvider.GetRequiredKeyedService<IMessageDecoder>(msgType);
            var ack = (AckPayload)decoder.Decode(body);
            messageType = ack.MessageTypeAck;
            return true;
        }
        catch
        {
            return false;
        }
    }
}
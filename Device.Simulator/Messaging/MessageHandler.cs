using System.Buffers;
using Device.Simulator.Configuration;
using Device.Simulator.Networking;
using Gateway.Protocol.MessageDecoding.Decoders.Messages;
using Gateway.Protocol.MessageEncoding.Base.Interfaces;
using Gateway.Protocol.Payloads;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Device.Simulator.Messaging;

public class MessageHandler(
    ILogger<MessageHandler> logger,
    IOptions<SimulatorOptions> options,
    IMessageEncoder<LoginPayload> loginEncoder,
    IMessageEncoder<HeartbeatPayload> heartbeatEncoder,
    AckMessageDecoderParser ackMessageDecoderParser,
    IMessageSender messageSender
) : IMessageHandler
{
    public async Task<bool> SendLoginAsync(DeviceConnectionContext context,
        CancellationToken cancellationToken = default)
    {
        var message = new LoginPayload(
            new BarcodePayload(context.DeviceBarcode),
            new TimestampPayload(DateTime.Now));

        try
        {
            bool success = false;
            int retryCount = 0;

            while (retryCount < 3)
            {
                var buffer = context.Writer.GetSpan(LoginPayload.FixedSize);
                var position = loginEncoder.Encode(buffer, message);
                success = await messageSender
                    .SendWithRetryAsync(position, context, messageName: "LOGIN", cancellationToken)
                    .ConfigureAwait(false);
                if (success)
                {
                    break;
                }

                retryCount++;
                logger.LogWarning("[{device}] [{type}] retry {retry}", context.DeviceBarcode, "LOGIN", retryCount);
            }

            return success;
        }
        catch (Exception ex) when (ex is OperationCanceledException or TaskCanceledException)
        {
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError("[{device}] Login failed: {message}\n {stackTrace} ", context.DeviceBarcode, ex.Message,
                ex.StackTrace);
            return false;
        }
    }

    public async Task SendHeartbeatLoopAsync(DeviceConnectionContext context,
        CancellationToken cancellationToken = default)
    {
        using var timer = new PeriodicTimer(options.Value.HeartbeatInterval);

        try
        {
            while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false))
            {
                if (options.Value.SimulateDisconnections && Random.Shared.NextDouble() < 0.01)
                {
                    logger.LogInformation("[{device}] simulating random drop", context.DeviceBarcode);
                    break;
                }

                bool success = false;
                int retryCount = 0;

                var payload = new HeartbeatPayload(new BarcodePayload(context.DeviceBarcode),
                    new TimestampPayload(DateTime.Now));

                while (retryCount < 3)
                {
                    var buffer = context.Writer.GetSpan(HeartbeatPayload.FixedSize);
                    var position = heartbeatEncoder.Encode(buffer, payload);
                    success = await messageSender
                        .SendWithRetryAsync(position, context, messageName: "HEARTBEAT", cancellationToken)
                        .ConfigureAwait(false);

                    if (success)
                    {
                        break;
                    }

                    retryCount++;
                    logger.LogWarning("[{device}] [{type}] retry {retry}", context.DeviceBarcode, "LOGIN", retryCount);
                }

                if (success)
                {
                    continue;
                }

                logger.LogWarning("[{device}] [TIMEOUT] Heartbeat ACK missing", context.DeviceBarcode);
                break; // close connection
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    public bool TryParseAckFrame(ref ReadOnlySequence<byte> buffer)
    {
        if (buffer.Length < AckPayload.FixedSize) return false;
        var reader = new SequenceReader<byte>(buffer);
        try
        {
            var ack = ackMessageDecoderParser.Decode(ref reader);
            buffer = buffer.Slice(reader.Position);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
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
        var payload = new LoginPayload(
            new BarcodePayload(context.DeviceBarcode),
            new TimestampPayload(DateTime.Now));

        var message = loginEncoder.Encode(payload);

        try
        {
            return await messageSender.SendLoginMessageAsync(message, context, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is OperationCanceledException or TaskCanceledException)
        {
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[{device}] Login failed ", context.DeviceBarcode);
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

                var payload = new HeartbeatPayload(new BarcodePayload(context.DeviceBarcode),
                    new TimestampPayload(DateTime.Now));

                var message = heartbeatEncoder.Encode(payload);

                var success = await messageSender.SendHeartbeatMessageAsync(message, context, cancellationToken)
                    .ConfigureAwait(false);

                if (success) continue;
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
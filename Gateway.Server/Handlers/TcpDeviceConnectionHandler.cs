using System.Buffers;
using System.IO.Pipelines;
using System.Net.Sockets;
using Gateway.Monitoring.Services;
using Gateway.Protocol.Enums;
using Gateway.Protocol.Extensions;
using Gateway.Protocol.MessageDecoding.Interfaces;
using Gateway.Protocol.MessageEncoding.Interfaces;
using Gateway.Protocol.Payloads.Messages;
using Gateway.Server.Configuration;
using Gateway.Server.Connections;
using Gateway.Server.Messaging;
using Kafka.Producer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gateway.Server.Handlers;

public class TcpDeviceConnectionHandler(
    ILogger<TcpDeviceConnectionHandler> logger,
    DeviceConnectionManager connectionManager,
    IOptions<DeviceConnectionOptions> deviceConnectionOptions,
    IMessageDispatcher messageDispatcher,
    IPacketDecoderParserHelper packetDecoderParserHelper,
    IPacketEncoderParserHelper packetEncoderParserHelper,
    IMetricsService metrics,
    TelemetryKafkaProducer kafkaProducer
) : IDeviceConnectionAcceptor
{
    public async Task AcceptClient(TcpClient client, CancellationToken cancellationToken = default)
    {
        client.NoDelay = true;
        var endpoint = client.Client.RemoteEndPoint?.ToString() ?? "Unknown";
        var context = new DeviceConnectionContext
        {
            RemoteEndPoint = endpoint,
            TcpClient = client,
            Reader = PipeReader.Create(client.GetStream()),
            Writer = PipeWriter.Create(client.GetStream())
        };

        var id = connectionManager.Add(context);
        await RunAsync(id, context, cancellationToken);
    }

    private async Task RunAsync(Guid id, DeviceConnectionContext context, CancellationToken cancellationToken)
    {
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        try
        {
            timeoutCts.CancelAfter(deviceConnectionOptions.Value.LoginTimeout);

            if (!await AuthenticateHandshakeAsync(context, timeoutCts.Token))
            {
                logger.LogWarning("Unauthorized connection attempt from {RemoteEndPoint}", context.RemoteEndPoint);
                return; // Drop the connection immediately
            }

            timeoutCts.CancelAfter(deviceConnectionOptions.Value.HeartbeatTimeout);

            var processingTask = messageDispatcher.StartProcessingAsync(context, timeoutCts.Token);

            await ReadLoop(context, timeoutCts).ConfigureAwait(false);

            context.DeviceChannel.Writer.TryComplete();

            await processingTask.ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                logger.LogInformation(message: "Connection for {RemoteEndPoint} closed due to server shutdown.",
                    context.RemoteEndPoint);
            }
            else
            {
                logger.LogWarning(message: "Connection timed out (No Login/Heartbeat) from {RemoteEndPoint}",
                    context.RemoteEndPoint);
            }
        }
        catch (IOException ex) when (ex.InnerException is SocketException
                                     {
                                         SocketErrorCode:
                                         SocketError.OperationAborted or
                                         SocketError.ConnectionReset or
                                         SocketError.ConnectionAborted
                                     })
        {
            if (cancellationToken.IsCancellationRequested)
            {
                logger.LogWarning(message: "Connection closed for {DeviceBarcode} during server shutdown.",
                    context.DeviceBarcode);
            }
            else
            {
                logger.LogWarning(message: "Device {DeviceBarcode} closed the connection (aborted/reset).",
                    context.DeviceBarcode);
            }
        }

        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected connection error at {Endpoint}", context.RemoteEndPoint);
        }
        finally
        {
            await connectionManager.RemoveAsync(id);
        }
    }

    private async Task<bool> AuthenticateHandshakeAsync(DeviceConnectionContext context,
        CancellationToken cancellationToken)
    {
        var result = await context.Reader.ReadAsync(cancellationToken);
        var buffer = result.Buffer;

        if (!buffer.IsEmpty)
            logger.LogHex(buffer, $"[{context.DeviceBarcode}] Tx:");

        var originalStart = buffer.Start;

        try
        {
            if (packetDecoderParserHelper.TryGetPayloadBytesFromPacket(
                    ref buffer, out var body, out var messageType))
            {
                if (messageType == MessageType.Login)
                {
                    var data = new ReadOnlySequence<byte>(body.ToArray());
                    var msg = new IncomingMessage(context, Data: data, messageType);

                    if (context.DeviceChannel.Writer.TryWrite(msg))
                        await SendAck(context, messageType, cancellationToken);

                    context.Reader.AdvanceTo(buffer.Start, buffer.End);

                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during JWT handshake");
        }

        context.Reader.AdvanceTo(originalStart, buffer.End);
        return false;
    }

    private async Task ReadLoop(DeviceConnectionContext context, CancellationTokenSource timeoutCts)
    {
        while (!timeoutCts.IsCancellationRequested)
        {
            var result = await context.Reader.ReadAsync(timeoutCts.Token).ConfigureAwait(false);
            var buffer = result.Buffer;

            if (!buffer.IsEmpty)
                logger.LogHex(buffer, $"[{context.DeviceBarcode}] Tx:");

            SequencePosition consumed = buffer.Start;
            SequencePosition examined = buffer.End;

            try
            {
                // snapshot the buffer before parsing so we can capture raw bytes on failure
                var rawBuffer = buffer;

                while (packetDecoderParserHelper.TryGetPayloadBytesFromPacket(
                           ref buffer, out var body, out var messageType))
                {
                    timeoutCts.CancelAfter(deviceConnectionOptions.Value.HeartbeatTimeout);

                    var data = new ReadOnlySequence<byte>(body.ToArray());
                    var msg = new IncomingMessage(context, Data: data, messageType);

                    if (context.DeviceChannel.Writer.TryWrite(msg))
                        await SendAck(context, messageType, timeoutCts.Token);
                }

                // if bytes remain unconsumed after the parse loop, the packet was bad
                // publish to dead letter so it is never silently lost
                if (!buffer.IsEmpty && rawBuffer.Length != buffer.Length)
                {
                    var badBytes = buffer.ToArray();
                    logger.LogWarning("[{DeviceBarcode}] Unparseable packet ({Bytes} bytes) → dead letter",
                        context.DeviceBarcode, badBytes.Length);

                    metrics.IncrementDeadLetterCount();

                    await kafkaProducer.PublishDeadLetterAsync(
                        deviceId: context.DeviceBarcode,
                        Convert.ToHexString(badBytes),
                        timeoutCts.Token);

                    // advance past the bad bytes so the loop doesn't re-process them
                    buffer = buffer.Slice(buffer.End);
                }

                consumed = buffer.Start;

                if (result.IsCompleted) break;
            }
            finally
            {
                context.Reader.AdvanceTo(consumed, examined);
            }
        }
    }

    private async Task SendAck(DeviceConnectionContext context, MessageType messageType,
        CancellationToken timeoutCtsToken)
    {
        var ackPayload = new AckMessagePayload(messageType);
        Span<byte> ackBuffer = context.Writer.GetSpan(ackPayload.FixedSize + 4);

        var bytesWritten = packetEncoderParserHelper.EncodePayloadBytesIntoPacket(ref ackBuffer, ackPayload);

        context.Writer.Advance(bytesWritten);
        await context.Writer.FlushAsync(timeoutCtsToken);
        logger.LogHex(new ReadOnlySequence<byte>(context.Writer.GetMemory()[..bytesWritten]),
            $"[{context.DeviceBarcode}] Rx:");
    }
}
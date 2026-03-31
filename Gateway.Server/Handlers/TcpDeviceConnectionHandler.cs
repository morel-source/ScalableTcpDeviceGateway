using System.Buffers;
using System.IO.Pipelines;
using System.Net.Sockets;
using Gateway.Protocol.Enums;
using Gateway.Protocol.Extensions;
using Gateway.Protocol.MessageDecoding.Interfaces;
using Gateway.Protocol.MessageEncoding.Interfaces;
using Gateway.Protocol.Payloads;
using Gateway.Server.Configuration;
using Gateway.Server.Connections;
using Gateway.Server.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gateway.Server.Handlers;

public class TcpDeviceConnectionHandler(
    ILogger<TcpDeviceConnectionHandler> logger,
    DeviceConnectionManager connectionManager,
    IOptions<DeviceConnectionOptions> deviceConnectionOptions,
    IMessageDispatcher messageDispatcher,
    IPacketDecoderParserHelper packetDecoderParserHelper,
    IPacketEncoderParserHelper packetEncoderParserHelper
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
                while (packetDecoderParserHelper.GetPayloadBytesFromPacket(ref buffer, out var body,
                           out var messageType))
                {
                    timeoutCts.CancelAfter(deviceConnectionOptions.Value.HeartbeatTimeout);

                    var data = new ReadOnlySequence<byte>(body.ToArray());
                    var msg = new IncomingMessage(context, Data: data, messageType);

                    if (context.DeviceChannel.Writer.TryWrite(msg))
                    {
                        await SendAck(context, messageType, timeoutCts.Token);
                    }
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
        var ackPayload = new AckPayload(messageType);
        Span<byte> ackBuffer = context.Writer.GetSpan(ackPayload.FixedSize + 4);

        var bytesWritten = packetEncoderParserHelper.EncodePayloadBytesIntoPacket(ref ackBuffer, ackPayload);

        context.Writer.Advance(bytesWritten);
        await context.Writer.FlushAsync(timeoutCtsToken);
        logger.LogHex(new ReadOnlySequence<byte>(context.Writer.GetMemory()[..bytesWritten]),
            $"[{context.DeviceBarcode}] Rx:");
    }
}
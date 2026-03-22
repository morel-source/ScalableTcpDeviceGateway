using System.Buffers;
using System.IO.Pipelines;
using System.Net.Sockets;
using Gateway.Monitoring.Services;
using Gateway.Protocol.Enums;
using Gateway.Protocol.Extensions;
using Gateway.Protocol.MessageEncoding.Base.Interfaces;
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
    IMessageEncoder<AckPayload> ackMessageEncoder,
    IOptions<DeviceConnectionOptions> deviceConnectionOptions,
    IMessageDispatcher messageDispatcher,
    IMetricsService metrics
) : IDeviceConnectionAcceptor
{
    public async Task AcceptClient(TcpClient client, CancellationToken cancellationToken = default)
    {
        client.NoDelay = true;
        var endpoint = client.Client.RemoteEndPoint?.ToString() ?? "unknown";
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
                logger.LogInformation("Connection for {endpoint} closed due to server shutdown.",
                    context.RemoteEndPoint);
            }
            else
            {
                logger.LogWarning("Connection timed out (No Login/Heartbeat) from {endpoint}",
                    context.RemoteEndPoint);
            }
        }
        catch (IOException ex) when (ex.InnerException is SocketException socketEx &&
                                     (socketEx.SocketErrorCode == SocketError.OperationAborted ||
                                      socketEx.SocketErrorCode == SocketError.ConnectionReset))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                logger.LogInformation("Connection closed for {device} during server shutdown.",
                    context.DeviceBarcode);
            }
            else
            {
                logger.LogWarning("Device {device} forcibly closed the connection (Connection Reset).",
                    context.DeviceBarcode);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Unexpected connection error at {endpoint}", context.RemoteEndPoint);
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

            if (logger.IsEnabled(LogLevel.Debug) && !buffer.IsEmpty)
            {
                // Only pay for the hex conversion if we are actually debugging
                logger.LogHex(result.Buffer, $"[{context.DeviceBarcode}] Tx:");
            }

            SequencePosition consumed = buffer.Start;
            SequencePosition examined = buffer.Start;

            try
            {
                while (TryParseFrame(ref buffer, out var frame))
                {
                    timeoutCts.CancelAfter(deviceConnectionOptions.Value.HeartbeatTimeout);

                    var msg = new IncomingMessage(context, Data: frame.ToArray());

                    if (context.DeviceChannel.Writer.TryWrite(msg))
                    {
                        var ack = ackMessageEncoder.Encode(new AckPayload());
                        await context.Writer.WriteAsync(ack, timeoutCts.Token);
                        await context.Writer.FlushAsync(timeoutCts.Token);
                        if (logger.IsEnabled(LogLevel.Debug))
                        {
                            // Only pay for the hex conversion if we are actually debugging
                            logger.LogHex(ack, $"[{context.DeviceBarcode}] Rx:");
                        }
                    }

                    consumed = buffer.Start;
                }

                if (result.IsCompleted) break;

                examined = buffer.End;
            }
            finally
            {
                context.Reader.AdvanceTo(consumed, examined);
            }
        }
    }

    private bool TryParseFrame(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> frame)
    {
        frame = default;

        // need at least 3 bytes to read the Header (Start, Type, Length)
        if (buffer.Length < 15) return false;

        // Peek at the Start Byte
        if (buffer.FirstSpan[0] != (byte)MessageType.StartByte)
        {
            buffer = buffer.Slice(1); // Syncing: Skip 1 byte and try again
            return false;
        }

        // Read the Length from the 3rd byte (Index 2)
        //int payloadLength = buffer.Slice(2, 1).FirstSpan[0];

        // Total size = Header (3 bytes) + Payload + EndByte (1 byte)
        //int totalMessageSize = 3 + payloadLength + 1; 

        // Do we have the full message yet?
        //if (buffer.Length < totalMessageSize) return false;

        int totalMessageSize = 15;
        // Validate the End Byte at the dynamic position
        if (buffer.Slice(totalMessageSize - 1, 1).FirstSpan[0] != (byte)MessageType.EndByte)
        {
            // If the end byte isn't where it's supposed to be, the stream is corrupted.
            // We skip the start byte to try and find the next valid frame.
            buffer = buffer.Slice(1);
            return false;
        }

        // SUCCESS: Slice the exact dynamic size
        frame = buffer.Slice(0, totalMessageSize);
        buffer = buffer.Slice(totalMessageSize);
        return true;
    }
}
using System.Buffers;
using Gateway.Protocol.Enums;
using Gateway.Protocol.MessageDecoding.Base.Interfaces;
using Gateway.Server.Connections;
using Gateway.Server.Handlers.Base;
using Microsoft.Extensions.Logging;

namespace Gateway.Server.Messaging;

public class MessageDispatcher(
    ILogger<MessageDispatcher> logger,
    IDictionary<MessageType, IHandler> handlers,
    IDictionary<MessageType, IMessageDecoder> parsers
) : IMessageDispatcher
{
    public async Task StartProcessingAsync(DeviceConnectionContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await foreach (var msg in context.DeviceChannel.Reader.ReadAllAsync(cancellationToken))
            {
                // No more 'using' block needed here
                var sequence = new ReadOnlySequence<byte>(msg.Data);

                if (IsValidFrame(sequence, out var messageType))
                {
                    if (parsers.TryGetValue(messageType, out var parser))
                    {
                        var reader = new SequenceReader<byte>(sequence);
                        var payload = parser.Decode(ref reader);

                        if (handlers.TryGetValue(messageType, out var handler))
                        {
                            await handler.TryProcessMessage(msg.Context, payload, cancellationToken);
                        }
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Processing loop stopped for device {device} (Connection closed or timed out).",
                context.DeviceBarcode ?? "Unknown");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing for {device}", context.DeviceBarcode ?? "Unknown");
        }
    }

    private bool IsValidFrame(ReadOnlySequence<byte> buffer, out MessageType messageType)
    {
        messageType = MessageType.Unknown;

        // Check start (First byte) 
        byte startByte = buffer.FirstSpan[0];

        // Check type (Index 1)
        byte typeByte = buffer.Slice(start: 1, length: 1).FirstSpan[0];
        if (Enum.IsDefined(typeof(MessageType), typeByte))
        {
            messageType = (MessageType)typeByte;
        }

        // Check end (Last byte)
        byte endByte = buffer.Slice(buffer.Length - 1).FirstSpan[0];

        return startByte == (byte)MessageType.StartByte && endByte == (byte)MessageType.EndByte;
    }
}
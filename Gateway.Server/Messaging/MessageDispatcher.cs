using Gateway.Protocol.Enums;
using Gateway.Protocol.MessageDecoding.Interfaces;
using Gateway.Server.Connections;
using Gateway.Server.Handlers.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gateway.Server.Messaging;

public class MessageDispatcher(
    ILogger<MessageDispatcher> logger,
    IServiceProvider serviceProvider
) : IMessageDispatcher
{
    public async Task StartProcessingAsync(DeviceConnectionContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await foreach (var msg in context.DeviceChannel.Reader.ReadAllAsync(cancellationToken))
            {
                if (msg.Data.IsEmpty || msg.MessageType == MessageType.Unknown)
                    continue;

                var decoder = serviceProvider.GetRequiredKeyedService<IMessageDecoder>(msg.MessageType);
                var payload = decoder.Decode(msg.Data);
                var handler = serviceProvider.GetRequiredKeyedService<IMessageHandler>(msg.MessageType);
                await handler.TryProcessMessage(context, payload, cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation(
                message: "Processing loop stopped for device {DeviceBarcode} (Connection closed or timed out).",
                context.DeviceBarcode);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error processing for {DeviceBarcode}", context.DeviceBarcode);
        }
    }
}
using Gateway.Server.Connections;

namespace Gateway.Server.Messaging;

public interface IMessageDispatcher
{
    Task StartProcessingAsync(DeviceConnectionContext context, CancellationToken cancellationToken = default);
}
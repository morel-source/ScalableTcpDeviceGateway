using Gateway.Protocol.Payloads.Base;
using Gateway.Server.Connections;

namespace Gateway.Server.Handlers.Base;

public interface IMessageHandler
{
    Task TryProcessMessage(DeviceConnectionContext context, IPayload payload,
        CancellationToken cancellation = default);
}
using Gateway.Protocol.Payloads;
using Gateway.Server.Connections;

namespace Gateway.Server.Handlers.Base;

public interface IHandler
{
    Task TryProcessMessage(DeviceConnectionContext context, IPayload payload,
        CancellationToken cancellation = default);
}
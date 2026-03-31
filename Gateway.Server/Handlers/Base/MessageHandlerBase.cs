using Gateway.Protocol.Payloads;
using Gateway.Server.Connections;

namespace Gateway.Server.Handlers.Base;

public abstract class MessageHandlerBase<TPayload> : IMessageHandler where TPayload : IPayload
{
    public async Task TryProcessMessage(DeviceConnectionContext context, IPayload payload,
        CancellationToken cancellationToken = default)
    {
        if (payload is TPayload typed)
            await ProcessMessage(context, typed, cancellationToken).ConfigureAwait(false);
    }

    protected abstract Task ProcessMessage(DeviceConnectionContext context, TPayload payload,
        CancellationToken cancellationToken = default);
}
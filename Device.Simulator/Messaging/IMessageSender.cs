using Device.Simulator.Networking;
using Gateway.Protocol.Enums;

namespace Device.Simulator.Messaging;

public interface IMessageSender
{
    Task<bool> SendWithRetryAsync(int position, DeviceConnectionContext context, MessageType messageType,
        CancellationToken cancellationToken = default);
}
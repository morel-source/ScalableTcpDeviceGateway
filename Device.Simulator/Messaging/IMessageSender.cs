using Device.Simulator.Networking;

namespace Device.Simulator.Messaging;

public interface IMessageSender
{
    Task<bool> SendWithRetryAsync(
        int position,
        DeviceConnectionContext context,
        string messageName,
        CancellationToken cancellationToken = default);
}
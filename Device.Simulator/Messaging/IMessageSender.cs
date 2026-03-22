using Device.Simulator.Networking;

namespace Device.Simulator.Messaging;

public interface IMessageSender
{
    Task<bool> SendLoginMessageAsync(byte[] message, DeviceConnectionContext context,
        CancellationToken cancellationToken = default);

    Task<bool> SendHeartbeatMessageAsync(byte[] message, DeviceConnectionContext context,
        CancellationToken cancellationToken = default);
}
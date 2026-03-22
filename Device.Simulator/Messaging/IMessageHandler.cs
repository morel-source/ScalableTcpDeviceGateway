using System.Buffers;
using Device.Simulator.Networking;

namespace Device.Simulator.Messaging;

public interface IMessageHandler
{
    Task<bool> SendLoginAsync(DeviceConnectionContext context, CancellationToken cancellationToken = default);
    Task SendHeartbeatLoopAsync(DeviceConnectionContext context, CancellationToken cancellationToken = default);
    bool TryParseAckFrame(ref ReadOnlySequence<byte> buffer);
}
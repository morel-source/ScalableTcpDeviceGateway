using System.Buffers;
using Device.Simulator.Messaging.Messages;
using Device.Simulator.Networking;
using Gateway.Protocol.Enums;

namespace Device.Simulator.Messaging;

public class MessageHandler(
    LoginMessageHandler loginMessageHandler,
    HeartbeatMessageHandler heartbeatMessageHandler,
    AlertMessageHandler alertMessageHandler,
    TelemetryMessageHandler telemetryMessageHandler,
    AckMessageHandler ackMessageHandler
) : IMessageHandler
{
    public async Task<bool> SendLoginAsync(DeviceConnectionContext context,
        CancellationToken cancellationToken = default)
    {
        return await loginMessageHandler.SendLoginAsync(context, cancellationToken);
    }

    public async Task SendHeartbeatLoopAsync(DeviceConnectionContext context,
        CancellationToken cancellationToken = default)
    {
        await heartbeatMessageHandler.SendHeartbeatLoopAsync(context, cancellationToken);
    }

    public async Task SendTelemetryLoopAsync(DeviceConnectionContext context,
        CancellationToken cancellationToken = default)
    {
        await telemetryMessageHandler.SendTelemetryLoopAsync(context, cancellationToken);
    }

    public async Task SendAlertAsync(DeviceConnectionContext context, AlertType alertType,
        CancellationToken cancellationToken = default)
    {
        await alertMessageHandler.SendAlertAsync(context, alertType, cancellationToken);
    }

    public bool TryParseAckFrame(ref ReadOnlySequence<byte> buffer, out MessageType messageType)
    {
        return ackMessageHandler.TryParseAckFrame(ref buffer, out messageType);
    }
}
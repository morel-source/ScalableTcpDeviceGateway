using Gateway.Protocol.Enums;

namespace Gateway.Protocol.Extensions;

public static class MessageTypeExtensions
{
    public static string GetName(this MessageType type) => type switch
    {
        MessageType.Login => nameof(MessageType.Login),
        MessageType.Heartbeat => nameof(MessageType.Heartbeat),
        MessageType.Ack => nameof(MessageType.Ack),
        MessageType.Telemetry => nameof(MessageType.Telemetry),
        MessageType.Alert => nameof(MessageType.Alert),
        _ => nameof(MessageType.Unknown),
    };
}
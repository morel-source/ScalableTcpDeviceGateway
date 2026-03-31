using Gateway.Protocol.Enums;

namespace Gateway.Protocol.Extensions;

public static class MessageTypeExtensions
{
    public static string GetName(this MessageType type) => type switch
    {
        MessageType.Heartbeat => nameof(MessageType.Heartbeat),
        MessageType.Login => nameof(MessageType.Login),
        MessageType.Ack => nameof(MessageType.Ack),
        _ => nameof(MessageType.Unknown),
    };
}
using Gateway.Protocol.Enums;

namespace Gateway.Protocol.Payloads;

public readonly record struct AckPayload(
    MessageType MessageTypeAck
) : IMessagePayload
{
    public int FixedSize => 1;
    public MessageType MessageType => MessageType.Ack;
}
using Gateway.Protocol.Enums;
using Gateway.Protocol.Payloads.Base;

namespace Gateway.Protocol.Payloads.Messages;

public readonly record struct AckMessagePayload(
    MessageType MessageTypeAck
) : IMessagePayload
{
    public int FixedSize => 1;
    public MessageType MessageType => MessageType.Ack;
}
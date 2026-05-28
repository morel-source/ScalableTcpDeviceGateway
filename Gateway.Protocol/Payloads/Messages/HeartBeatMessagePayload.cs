using Gateway.Protocol.Enums;
using Gateway.Protocol.Payloads.Base;
using Gateway.Protocol.Payloads.Fields;

namespace Gateway.Protocol.Payloads.Messages;

public readonly record struct HeartbeatMessagePayload(
    BarcodePayload DeviceBarcode,
    TimestampPayload Timestamp
) : IMessagePayload
{
    public int FixedSize => DeviceBarcode.FixedSize + Timestamp.FixedSize;
    public MessageType MessageType => MessageType.Heartbeat;
}
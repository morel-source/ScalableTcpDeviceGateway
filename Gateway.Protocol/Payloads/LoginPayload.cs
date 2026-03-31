using Gateway.Protocol.Enums;

namespace Gateway.Protocol.Payloads;

public readonly record struct LoginPayload(
    BarcodePayload DeviceBarcode,
    TimestampPayload Timestamp
) : IMessagePayload
{
    public int FixedSize => DeviceBarcode.FixedSize + Timestamp.FixedSize;
    public MessageType MessageType => MessageType.Login;
}
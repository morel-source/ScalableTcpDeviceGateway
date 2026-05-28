using Gateway.Protocol.Enums;
using Gateway.Protocol.Payloads.Base;
using Gateway.Protocol.Payloads.Fields;

namespace Gateway.Protocol.Payloads.Messages;

public readonly record struct AlertMessagePayload(
    BarcodePayload DeviceBarcode,
    AlertTypePayload AlertType,
    TimestampPayload Timestamp
) : IMessagePayload
{
    public AlertMessagePayload(string deviceBarcode, AlertType alertType, DateTime timestamp)
        : this(new BarcodePayload(deviceBarcode), new AlertTypePayload(alertType), new TimestampPayload(timestamp))
    {
    }

    public int FixedSize =>
        DeviceBarcode.FixedSize +
        AlertType.FixedSize +
        Timestamp.FixedSize;

    public MessageType MessageType => MessageType.Alert;
}
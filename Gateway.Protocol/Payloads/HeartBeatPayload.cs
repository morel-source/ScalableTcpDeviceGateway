namespace Gateway.Protocol.Payloads;

public readonly record struct HeartbeatPayload(
    BarcodePayload DeviceBarcode,
    TimestampPayload Timestamp
) : IPayload
{
    public static int FixedSize =>
        1 + // startByte
        1 + // messageTypeByte
        BarcodePayload.FixedSize +
        TimestampPayload.FixedSize +
        1; // endByte
}
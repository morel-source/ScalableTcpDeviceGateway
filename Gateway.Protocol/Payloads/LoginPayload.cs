namespace Gateway.Protocol.Payloads;

public readonly record struct LoginPayload(
    BarcodePayload DeviceBarcode,
    TimestampPayload Timestamp
) : IPayload
{
    public static int FixedSize =>
        1 +
        1 +
        BarcodePayload.FixedSize +
        TimestampPayload.FixedSize +
        1;
}

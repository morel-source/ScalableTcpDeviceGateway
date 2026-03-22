namespace Gateway.Protocol.Payloads;

public readonly record struct BarcodePayload(string Barcode) : IPayload
{
    public static int FixedSize => 6;
}
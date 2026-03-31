using System.Text;

namespace Gateway.Protocol.Payloads;

public readonly record struct BarcodePayload : IPayload
{
    private readonly ReadOnlyMemory<byte> _data;

    public BarcodePayload(ReadOnlyMemory<byte> data)
    {
        _data = data;
        Value = null;
    }

    public BarcodePayload(string barcode)
    {
        Value = barcode;
        _data = Encoding.ASCII.GetBytes(barcode);
    }

    public string Value => field ?? Encoding.ASCII.GetString(_data.Span);

    public ReadOnlySpan<byte> AsSpan() => _data.Span;

    public bool Equals(BarcodePayload other) =>
        string.Equals(Value, other.Value, StringComparison.Ordinal);

    public override int GetHashCode() =>
        Value?.GetHashCode(StringComparison.Ordinal) ?? 0;

    public int FixedSize => 6;
}
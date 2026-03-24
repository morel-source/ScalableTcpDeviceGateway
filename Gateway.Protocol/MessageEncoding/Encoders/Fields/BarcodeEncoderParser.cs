using Gateway.Protocol.MessageEncoding.Base.Interfaces;
using Gateway.Protocol.Payloads;

namespace Gateway.Protocol.MessageEncoding.Encoders.Fields;

public class BarcodeEncoderParser : IFieldEncoder<BarcodePayload>
{
    public void Encode(Span<byte> buffer, BarcodePayload payload, ref int position)
    {
        ReadOnlySpan<byte> barcodeBytes = payload.AsSpan();

        if (barcodeBytes.Length != BarcodePayload.FixedSize)
            throw new InvalidDataException("Invalid Barcode length for encoding");

        barcodeBytes.CopyTo(buffer.Slice(start: position, length: BarcodePayload.FixedSize));
        position += BarcodePayload.FixedSize;
    }
}
using Gateway.Protocol.MessageEncoding.Interfaces;
using Gateway.Protocol.Payloads;

namespace Gateway.Protocol.MessageEncoding.Encoders.Fields;

public class BarcodeEncoderParser : IFieldEncoder<BarcodePayload>
{
    public void Encode(ref Span<byte> buffer, BarcodePayload payload, ref int position)
    {
        ReadOnlySpan<byte> barcodeBytes = payload.AsSpan();

        if (barcodeBytes.Length != payload.FixedSize)
            throw new InvalidDataException("Invalid Barcode length for encoding");

        barcodeBytes.CopyTo(buffer.Slice(start: position, length: payload.FixedSize));
        position += payload.FixedSize;
    }
}
using System.Text;
using Gateway.Protocol.MessageEncoding.Base.Interfaces;
using Gateway.Protocol.Payloads;

namespace Gateway.Protocol.MessageEncoding.Encoders.Fields;

public class BarcodeEncoderParser : IFieldEncoder<BarcodePayload>
{
    public void Encode(Span<byte> buffer, BarcodePayload payload, ref int position)
    {
        Encoding.ASCII.GetBytes(payload.Barcode, buffer.Slice(position, BarcodePayload.FixedSize));
        position += BarcodePayload.FixedSize;
    }
}
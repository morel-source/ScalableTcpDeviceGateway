using System.Buffers;
using System.Text;
using Gateway.Protocol.MessageDecoding.Base.Interfaces;
using Gateway.Protocol.Payloads;

namespace Gateway.Protocol.MessageDecoding.Decoders.Fields;

public sealed class BarcodeDecoderParser : IFieldDecoder<BarcodePayload>
{
    public BarcodePayload Decode(ref SequenceReader<byte> reader)
    {
        if (reader.Remaining < BarcodePayload.FixedSize)
            throw new InvalidDataException("Buffer too small for Barcode");
        
        ReadOnlySpan<byte> span = reader.UnreadSpan.Slice(0, BarcodePayload.FixedSize);
        string barcode = Encoding.ASCII.GetString(span);
        reader.Advance(BarcodePayload.FixedSize);
        return new BarcodePayload(barcode);
    }
}
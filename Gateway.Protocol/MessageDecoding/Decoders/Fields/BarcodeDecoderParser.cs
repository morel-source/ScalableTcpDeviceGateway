using System.Buffers;
using Gateway.Protocol.MessageDecoding.Base.Interfaces;
using Gateway.Protocol.Payloads;

namespace Gateway.Protocol.MessageDecoding.Decoders.Fields;

public sealed class BarcodeDecoderParser : IFieldDecoder<BarcodePayload>
{
    public BarcodePayload Decode(ref SequenceReader<byte> reader)
    {
        if (reader.Remaining < BarcodePayload.FixedSize)
            throw new InvalidDataException("Buffer too small for Barcode");

        var barcodeSlice = reader.Sequence.Slice(reader.Position, BarcodePayload.FixedSize);
        reader.Advance(BarcodePayload.FixedSize);
        return new BarcodePayload(barcodeSlice.First);
    }
}
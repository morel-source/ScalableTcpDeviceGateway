using System.Buffers;
using Gateway.Protocol.MessageDecoding.Base.Interfaces;
using Gateway.Protocol.Payloads;

namespace Gateway.Protocol.MessageDecoding.Decoders.Fields;

public sealed class TimestampDecoderParser : IFieldDecoder<TimestampPayload>
{
    public TimestampPayload Decode(ref SequenceReader<byte> reader)
    {
        if (!(
                reader.TryRead(out byte year) &&
                reader.TryRead(out byte month) &&
                reader.TryRead(out byte day) &&
                reader.TryRead(out byte hour) &&
                reader.TryRead(out byte minute) &&
                reader.TryRead(out byte second)
            ))
            throw new InvalidDataException();

        return new TimestampPayload(new DateTime(year + 2000, month, day, hour, minute, second));
    }
}
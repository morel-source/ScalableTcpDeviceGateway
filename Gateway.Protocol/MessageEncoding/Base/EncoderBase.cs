using Gateway.Protocol.MessageEncoding.Base.Interfaces;
using Gateway.Protocol.Payloads;

namespace Gateway.Protocol.MessageEncoding.Base;

public abstract class EncoderBase<TPayload> : IMessageEncoder<TPayload>
    where TPayload : IPayload
{
    public int Encode(Span<byte> buffer, TPayload payload)
    {
        var position = 0;
        Encode(ref buffer, payload, ref position);
        return position;
    }

    protected abstract void Encode(ref Span<byte> buffer, TPayload payload, ref int position);
}
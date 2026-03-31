using Gateway.Protocol.MessageEncoding.Interfaces;
using Gateway.Protocol.Payloads;

namespace Gateway.Protocol.MessageEncoding.Base;

public abstract class EncoderBase<TPayload> : IMessageEncoder
    where TPayload : IMessagePayload
{
    public void Encode(ref Span<byte> buffer, IMessagePayload payload, ref int position)
    {
        if (payload is not TPayload typedPayload)
        {
            throw new ArgumentException($"Payload must be of type {typeof(TPayload).Name}", nameof(payload));
        }

        Encode(ref buffer, typedPayload, ref position);
    }

    protected abstract void Encode(ref Span<byte> buffer, TPayload payload, ref int position);
}
using System.Buffers;
using Gateway.Protocol.MessageEncoding.Base.Interfaces;
using Gateway.Protocol.Payloads;

namespace Gateway.Protocol.MessageEncoding.Base;

public abstract class EncoderBase<TPayload> : IMessageEncoder<TPayload>
    where TPayload : IPayload
{
    public byte[] Encode(TPayload payload)
    {
        // Allocation-free buffer from the shared pool
        // Note: ArrayPool may return an array LARGER than FixedSize
        var buffer = ArrayPool<byte>.Shared.Rent(TPayload.FixedSize);
        var position = 0;

        try
        {
            Encode(ref buffer, payload, ref position);

            // Because our Networking layer expects the exact size, 
            // we have to return a trimmed array. 
            // In a high-perf scenario, it's better to return ReadOnlySpan<byte> 
            // instead of byte[] to avoid this last allocation.
            var result = new byte[TPayload.FixedSize];
            Array.Copy(buffer, result, TPayload.FixedSize);
            return result;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    protected abstract void Encode(ref byte[] buffer, TPayload payload, ref int position);
}
using Gateway.Protocol.Payloads.Base;

namespace Gateway.Protocol.MessageEncoding.Interfaces;

public interface IMessageEncoder
{
    void Encode(ref Span<byte> buffer, IMessagePayload payload, ref int position);
}
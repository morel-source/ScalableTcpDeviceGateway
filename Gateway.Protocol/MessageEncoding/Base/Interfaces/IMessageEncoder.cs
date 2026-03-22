using Gateway.Protocol.Payloads;

namespace Gateway.Protocol.MessageEncoding.Base.Interfaces;

public interface IMessageEncoder<in TPayload> where TPayload : IPayload
{
    byte[] Encode(TPayload payload);
}
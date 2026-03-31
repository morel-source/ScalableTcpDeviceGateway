using Gateway.Protocol.Enums;

namespace Gateway.Protocol.Payloads;

public interface IPayload
{
    int FixedSize { get; }
}

public interface IMessagePayload : IPayload
{
    MessageType MessageType { get; }
}
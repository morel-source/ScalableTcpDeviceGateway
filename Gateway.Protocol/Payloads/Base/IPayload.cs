using Gateway.Protocol.Enums;

namespace Gateway.Protocol.Payloads.Base;

public interface IPayload
{
    int FixedSize { get; }
}

public interface IMessagePayload : IPayload
{
    MessageType MessageType { get; }
}
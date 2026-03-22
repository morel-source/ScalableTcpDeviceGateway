namespace Gateway.Protocol.Payloads;

public interface IPayload
{
    static abstract int FixedSize { get; }
}

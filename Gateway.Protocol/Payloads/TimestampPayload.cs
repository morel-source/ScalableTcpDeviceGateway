namespace Gateway.Protocol.Payloads;

public readonly record struct TimestampPayload(DateTime Timestamp) : IPayload
{
    public int FixedSize => 6;
}

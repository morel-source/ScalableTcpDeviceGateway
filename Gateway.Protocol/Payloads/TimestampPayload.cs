namespace Gateway.Protocol.Payloads;

public readonly record struct TimestampPayload(DateTime Timestamp) : IPayload
{
    public static int FixedSize => 6;
}

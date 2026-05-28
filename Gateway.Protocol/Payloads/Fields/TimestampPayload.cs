using Gateway.Protocol.Payloads.Base;

namespace Gateway.Protocol.Payloads.Fields;

public readonly record struct TimestampPayload(DateTime Timestamp) : IPayload
{
    public int FixedSize => 6;
}
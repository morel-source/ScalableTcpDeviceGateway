using Gateway.Protocol.Payloads.Base;

namespace Gateway.Protocol.Payloads.Fields;

public readonly record struct SignalPayload(byte Strength) : IPayload
{
    public int FixedSize => 1;
}
using Gateway.Protocol.Payloads.Base;

namespace Gateway.Protocol.Payloads.Fields;

public readonly record struct BatteryPayload(byte Percentage) : IPayload
{
    public int FixedSize => 1;
}
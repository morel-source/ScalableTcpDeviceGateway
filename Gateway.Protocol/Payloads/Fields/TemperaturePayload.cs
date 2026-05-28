using Gateway.Protocol.Payloads.Base;

namespace Gateway.Protocol.Payloads.Fields;

public readonly record struct TemperaturePayload(double Celsius) : IPayload
{
    public int FixedSize => 2; // stored as short (temp * 10)
}
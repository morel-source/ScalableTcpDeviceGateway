namespace Gateway.Protocol.Payloads;

public readonly record struct AckPayload : IPayload
{
    public static int FixedSize => 3;
}
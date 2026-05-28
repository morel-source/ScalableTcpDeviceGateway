using Gateway.Protocol.Enums;
using Gateway.Protocol.Payloads.Base;

namespace Gateway.Protocol.Payloads.Fields;

public readonly record struct AlertTypePayload(AlertType AlertType) : IPayload
{
    public int FixedSize => 1;
}
namespace Gateway.Protocol.Enums;

public enum AlertType : byte
{
    HighTemperature = 0x01,
    LowBattery      = 0x02,
    SignalLost      = 0x03,
}
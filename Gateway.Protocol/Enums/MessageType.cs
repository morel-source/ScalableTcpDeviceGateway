namespace Gateway.Protocol.Enums;

public enum MessageType : byte
{
    Unknown,
    Login = 0x01,
    Heartbeat = 0x02,
    Ack = 0x03,
    Telemetry = 0x04,
    Alert = 0x05
}

public enum FrameByte : byte
{
    StartByte = 0x02,
    EndByte = 0x03,
}